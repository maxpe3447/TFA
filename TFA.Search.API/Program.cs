using TFA.API.DependencyInjection;
using TFA.Search.API.Controllers;
using TFA.Search.API.Grpc;
using TFA.Search.API.Monitoring;
using TFA.Search.Domain.DependencyInjection;
using TFA.Search.Storage.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseKestrel(options =>
//{
//    options.AllowSynchronousIO = true;
//    options.ListenAnyIP(7161, configure =>
//    {
//        configure.UseHttps();
//        configure.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
//    });

//    options.ListenAnyIP(5144, configure =>
//    {
//        configure.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
//    });
//});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services
    .AddApiLogging(builder.Configuration, builder.Environment)
    .AddApiMetrics(builder.Configuration);

builder.Services
    .AddSearchDomain()
    .AddSearchStorage(builder.Configuration.GetConnectionString("SearchIndex")!);

builder.Services.AddGrpcReflection().AddGrpc();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGrpcService<SearchEngineGrpcService>();
app.MapGrpcReflectionService();

app.Run();
