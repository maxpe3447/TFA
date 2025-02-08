
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using TFA.Search.API.Grpc;

namespace TFA.Search.ForumConsumer;

public class ForumSearchConsumer(
    IConsumer<byte[], byte[]> consumer,
    SearchEngine.SearchEngineClient client,
    IOptions<ConsumerConfig> consumerConfig) : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new("ForumSearchConsumer");

    public readonly ConsumerConfig _consumerConfig = consumerConfig.Value!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        consumer.Subscribe("tfa.DomainEvents");
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            if (consumeResult is not { IsPartitionEOF: false })
            {
                await Task.Delay(300, stoppingToken);
                continue;
            }

            //var activityId = consumeResult.Message.Headers.TryGetLastBytes("activity_id", out var lastBytes)
            //    ? Encoding.UTF8.GetString(lastBytes)
            //    : null;

            var domainEventWrapper = JsonSerializer.Deserialize<DomainEventWrapper>(consumeResult.Message.Value);
            var contentBlob = Convert.FromBase64String(domainEventWrapper!.ContentBlob);
            var domainEvent = JsonSerializer.Deserialize<ForumDomainEvent>(contentBlob);

            string test = Encoding.UTF8.GetString(consumeResult.Message.Value);

            using var activity = ActivitySource.StartActivity(
                "ForumSearchConsumer.Kafka.Consumer",
                ActivityKind.Consumer,
                ActivityContext.TryParse(/*activityId*/ domainEventWrapper.ActivityId, null, out var context) ? context : default);

            activity?.AddTag("messaging.system", "kafka");
            activity?.AddTag("messaging.destination.name", "tfa.DomainEvents");
            activity?.AddTag("messaging.kafka.consumer_group", consumerConfig.Value.GroupId);
            activity?.AddTag("messaging.kafka.partition", consumeResult.Partition);


            switch (domainEvent.EventType)
            {
                case ForumDomainEventType.TopicCreated:
                    await client.IndexAsync(new IndexRequest
                    {
                        Id = domainEvent.TopicId.ToString(),
                        Type = SearchEntityType.ForumTopic,
                        Title = domainEvent.Title,
                    }, cancellationToken: stoppingToken);
                    break;
                case ForumDomainEventType.CommentCreated:
                    await client.IndexAsync(new IndexRequest
                    {
                        Id = domainEvent.Comment!.CommentId.ToString(),
                        Type = SearchEntityType.ForumComment,
                        Text = domainEvent.Comment.Text,
                    }, cancellationToken: stoppingToken);
                    break;
                default:
                    throw new ArgumentException();
            }


            consumer.Commit(consumeResult);
        }
        consumer.Close();
    }
}
