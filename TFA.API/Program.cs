using Microsoft.EntityFrameworkCore;
using TFA.Storage;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<ForumDbContext>(opt=>opt
.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"), b=>b.MigrationsAssembly("TFA.API")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

//app.Services.GetRequiredService<ForumDbContext>().Database.Migrate();

app.Run();
