using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;
using TFA.API.Middlewares;
using TFA.Domain.DependencyInjection;
using TFA.Storage.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(b => b.AddSerilog(new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.WithProperty("Application", "TFA.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.OpenSearch(
        builder.Configuration.GetConnectionString("Logs"),
        "forum-logs-{0:yyyy.MM.dd}"))
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.Console())
    .CreateLogger()));

builder.Services
    .AddForumDomain()
    .AddForumStorage(builder.Configuration.GetConnectionString("Postgres")??"");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ErrorHandlerMiddleware>();
//using var scope = app.Services.CreateScope();
//var service = scope.ServiceProvider;

//var context = service.GetRequiredService<ForumDbContext>();
//bool res = context.Forums.Any();
//if (!res)
//{
//    context.Forums.Add(new TFA.Storage.Entities.Forum { ForumId = Guid.Parse("8f54d8f5-6495-4818-8956-e735867469b9".ToUpper()), Title = "Blog"});
//    await context.SaveChangesAsync();
//}


app.Run();
