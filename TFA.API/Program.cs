using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;
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

var mapper = app.Services.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();

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


public partial class Program { }