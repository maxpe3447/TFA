using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;
using System.Runtime.InteropServices;
using TFA.API.Middlewares;
using TFA.Domain;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Models;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Storage;
using TFA.Storage.Storages;

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

builder.Services.AddScoped<ICreateTopicUseCase, CreateTopicUseCase>();
builder.Services.AddScoped<IGetForumsUseCase, GetForumsUseCase>();
builder.Services.AddScoped<ICreateTopicStorage, CreateTopicStorage>();
builder.Services.AddScoped<IIntentionResolver, TopicIntentionalResolver>();
builder.Services.AddScoped<IGetForumsStorage, GetForumStorage>();
builder.Services.AddScoped<IIntentionManager, IntentionManager>();
builder.Services.AddScoped<IIdentityProvider, IdentityProvider>();

builder.Services.AddScoped<IGuidFactory, GuidFactory>();
builder.Services.AddScoped<IMomentProvider, MomentProvider>();

//builder.Services.AddScoped<IValidator<CreateTopicCommand>, CreateTopicCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<Forum>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<ForumDbContext>(opt=> opt
    .UseNpgsql(builder.Configuration.GetConnectionString("Postgres"), b=>b.MigrationsAssembly("TFA.API")));

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
