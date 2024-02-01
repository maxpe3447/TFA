using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TFA.API.DependencyInjection;
using TFA.API.Middlewares;
using TFA.Domain.DependencyInjection;
using TFA.Storage.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiLogging(builder.Configuration, builder.Environment);

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.Run();


public partial class Program { }