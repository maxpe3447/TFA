using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ICreateTopicUseCase, CreateTopicUseCase>();
builder.Services.AddScoped<IGetForumsUseCase, GetForumsUseCase>();
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
using var scope = app.Services.CreateScope();
var service = scope.ServiceProvider;

service.GetRequiredService<ForumDbContext>().Database.Migrate();

app.Run();
