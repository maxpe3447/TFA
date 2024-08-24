
using Confluent.Kafka;
using System.Text.Json;
using TFA.Search.API.Grpc;

namespace TFA.Search.ForumConsumer;

public class ForumSearchConsumer(
    IConsumer<byte[], byte[]> consumer,
    SearchEngine.SearchEngineClient client) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        consumer.Subscribe("tfa.DomainEvents");
        while (stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            if(consumeResult is not { IsPartitionEOF: false})
            {
                await Task.Delay(300, stoppingToken);
                continue;
            }

            var domainEvent = JsonSerializer.Deserialize<DomainEvent>(consumeResult.Message.Value);
            var contentBlob = Convert.FromBase64String(domainEvent.ContentBlob);
            var topic = JsonSerializer.Deserialize<Topic>(contentBlob);

            await client.IndexAsync(new IndexRequest
            {
                Id = topic.Id.ToString(),
                Type = SearchEntityType.ForumTopic,
                Title = topic.Title,
            }, cancellationToken: stoppingToken);
            consumer.Commit(consumeResult);
        }
        consumer.Close();
    }
    private class DomainEvent
    {
        public string ContentBlob { get; set; }
    }
    private class Topic
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
