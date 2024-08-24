
using Confluent.Kafka;
using System.Text.Json;
using TFA.Forum.Domain.Models;

namespace TFA.API;

public class KafkaConsumer(
    //IConsumer<byte[], byte[]> consumer,
    //ILogger<KafkaConsumer> logger
    IServiceProvider serviceProvider
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await Task.Yield();

        var logger = serviceProvider.GetRequiredService<ILogger<KafkaConsumer>>();
        var consumer = serviceProvider.GetRequiredService<IConsumer<byte[], byte[]>>();

        logger.LogInformation("Subscribing to topic...");
        consumer.Subscribe("tfa.DomainEvents");
        //consumer.Subscribe("tfa.DomainEvents");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                if (consumeResult is null || consumeResult.IsPartitionEOF)
                {
                    await Task.Delay(1000);
                    continue;
                }
                var domainEvent = JsonSerializer.Deserialize<DomainEvent>(consumeResult.Message.Value)!;

                var contentBlob = Convert.FromBase64String(domainEvent.ContentBlob);
                var topic = JsonSerializer.Deserialize<Topic>(contentBlob);
                logger.LogInformation("Message about topic {TopicId} received", topic.Id);

                consumer.Commit(consumeResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }
        consumer.Close();
    }

    public class DomainEvent
    {
        public string ContentBlob { get; set; }
    }
}
