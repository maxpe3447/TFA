using Microsoft.Extensions.DependencyInjection;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain;
using TFA.Storage.Storages;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCases.GetTopics;

namespace TFA.Storage.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumStorage(this IServiceCollection services, string connectionString)
    {
        return services
            .AddScoped<IGetForumsStorage, GetForumStorage>()
            .AddScoped<ICreateTopicStorage, CreateTopicStorage>()
            .AddScoped<IGetTopicsStorage, GetTopicsStorage>()
            .AddScoped<IGuidFactory, GuidFactory>()
            .AddScoped<IMomentProvider, MomentProvider>()
            .AddDbContextPool<ForumDbContext>(opt => opt
                .UseNpgsql(connectionString, b => b.MigrationsAssembly("TFA.API")));

    }
}
