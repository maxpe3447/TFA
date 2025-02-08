using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TFA.API;
using TFA.API.IAuthentication;
using TFA.API.Middleware;
using TFA.API.Middlewares;
using TFA.API.Monitoring;
using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.DependencyInjection;
using TFA.Forum.Storage.DependencyInjection;
using TFA.Search.API.Grpc;
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiLogging(builder.Configuration, builder.Environment)
    .AddApiMetrics(builder.Configuration);
builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("Authentication").Bind);
builder.Services.AddScoped<IAuthTokenStorage, AuthTokenStorage>();

builder.Services
    .AddForumDomain()
    .AddForumStorage(builder.Configuration.GetConnectionString("Postgres") ?? "");

builder.Services
    .AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));
//.AddAutoMapper(config => config.AddProfile<ApiProfile>());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpcClient<SearchEngine.SearchEngineClient>(options => 
options.Address = new Uri(builder.Configuration.GetValue<string>("SearchEngine:Path")!));

//builder.Services.AddSingleton(new ConsumerBuilder<byte[], byte[]>(new ConsumerConfig
//{
//    BootstrapServers = "localhost:9092",
//    EnableAutoCommit = false,
//    AutoOffsetReset = AutoOffsetReset.Earliest,
//    GroupId = "tfa.experiment"
//}).Build());
//builder.Services.AddHostedService<KafkaConsumer>();
//builder.Services.Configure<HostOptions>(options =>
//    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost);

var app = builder.Build();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();
//app.UseWhen(ctx => ctx.Request.Headers.Select(x=>x.Key).Contains("TFA-Auth-Token"),
//    configuration => configuration.MapPrometheusScrapingEndpoint());
app.MapPrometheusScrapingEndpoint();

app.Run();


public partial class Program { }