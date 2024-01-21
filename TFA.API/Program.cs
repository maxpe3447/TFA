using Microsoft.EntityFrameworkCore;
using TFA.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<ForumDbContext>(opt=>opt
.UseNpgsql("User ID=postgres;Password=admin;host=localhost;Port=5432;Database=tfa;Pooling=true;MinPoolSize=0;MaxPoolSize=100;Connection Idle Lifetime=60;", b=>b.MigrationsAssembly("TFA.API")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

//app.Services.GetRequiredService<ForumDbContext>().Database.Migrate();

app.Run();
