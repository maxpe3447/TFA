using Microsoft.Extensions.DependencyInjection;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain;
using TFA.Storage.Storages;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCases.GetTopics;
using TFA.Domain.UseCases.CreateForum;
using System.Reflection;

namespace TFA.Storage.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumStorage(this IServiceCollection services, string connectionString)
    {
        services
            .AddScoped<ICreateForumStorage, CreateForumStorage>()
            .AddScoped<IGetForumsStorage, GetForumStorage>()
            .AddScoped<ICreateTopicStorage, CreateTopicStorage>()
            .AddScoped<IGetTopicsStorage, GetTopicsStorage>()
            .AddScoped<IGuidFactory, GuidFactory>()
            .AddScoped<IMomentProvider, MomentProvider>()
            .AddDbContextPool<ForumDbContext>(opt => opt
                .UseNpgsql(connectionString, b => b.MigrationsAssembly("TFA.API")));

        services
            .AddMemoryCache();

        services
            .AddAutoMapper(config => config.AddMaps(Assembly.GetAssembly(typeof(ForumDbContext))));
        return services;
    }
}
