using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TFA.Domain;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Models;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Storage;
using TFA.Storage.Storages;

var builder = WebApplication.CreateBuilder(args);

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
//builder.Services.AddDbContextPool<ForumDbContext>(opt=>opt
builder.Services.AddDbContext<ForumDbContext>(opt=> opt
.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"), b=>b.MigrationsAssembly("TFA.API")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
