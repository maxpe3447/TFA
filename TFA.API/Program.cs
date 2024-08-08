 using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using TFA.API.DependencyInjection;
using TFA.API.IAuthentication;
using TFA.API.Middlewares;
using TFA.API.Monitoring;
using TFA.Domain.Authentication;
using TFA.Domain.DependencyInjection;
using TFA.Storage.DependencyInjection;
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