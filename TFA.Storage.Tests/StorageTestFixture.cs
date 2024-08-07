
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using Testcontainers.PostgreSql;

namespace TFA.Storage.Tests;

public class StorageTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder().Build();

    public ForumDbContext GetDbContext() => new(new DbContextOptionsBuilder<ForumDbContext>()
            .UseNpgsql(
                _container.GetConnectionString(), 
                b => b.MigrationsAssembly("TFA.Storage")
            ).Options);

    public IMapper GetMapper() => new Mapper(new MapperConfiguration(cfg =>
    cfg.AddMaps(Assembly.GetAssembly(typeof(ForumDbContext)))));
    public IMemoryCache GetMemoryCache() => new MemoryCache(new MemoryCacheOptions());
    public async new Task DisposeAsync() => await _container.DisposeAsync();

    public virtual async Task InitializeAsync()
    {
        await _container.StartAsync();
        var forumDbContext = new ForumDbContext(new DbContextOptionsBuilder<ForumDbContext>()
            .UseNpgsql(_container.GetConnectionString(), b => b.MigrationsAssembly("TFA.Storage")).Options);
        await forumDbContext.Database.MigrateAsync();
    }
}
