using Confluent.Kafka;
using Microsoft.Extensions.Options;
using TFA.Search.API.Grpc;
using TFA.Search.ForumConsumer;
using TFA.Search.ForumConsumer.Monitoring;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<SearchEngine.SearchEngineClient>(opt =>
    opt.Address = new Uri(builder.Configuration.GetConnectionString("SearchEngine")!)
);

builder.Services
    .AddApiLogging(builder.Configuration, builder.Environment)
    .AddApiMetrics(builder.Configuration);

builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("Kafka").Bind);
builder.Services.AddSingleton(sp => new ConsumerBuilder<byte[], byte[]>(
    sp.GetRequiredService<IOptions<ConsumerConfig>>().Value).Build());

builder.Services.AddHostedService<ForumSearchConsumer>();

var app = builder.Build();


app.Run();
