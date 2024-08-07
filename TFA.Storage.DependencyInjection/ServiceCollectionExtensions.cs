using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TFA.Domain.Authentication;
using TFA.Domain.UseCases.CreateForum;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain.UseCases.GetTopics;
using TFA.Domain.UseCases.SignIn;
using TFA.Domain.UseCases.SignOn;
using TFA.Domain.UseCases.SignOut;
using TFA.Storage.Storages;

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
            .AddScoped<ISignOnStorage, SignOnStorage>()
            .AddScoped<ISignInStorage, SignInStorage>()
            .AddScoped<ISignOutStorage, SignOutStorage>()
            .AddScoped<IAuthenticationStorage, AuthenticationStorage>()
            .AddScoped<IGuidFactory, GuidFactory>()
            .AddScoped<IMomentProvider, MomentProvider>()
            .AddDbContextPool<ForumDbContext>(opt => opt
                .UseNpgsql(connectionString));

        services
            .AddMemoryCache();

        services
            .AddAutoMapper(config => config.AddMaps(Assembly.GetAssembly(typeof(ForumDbContext))));
        return services;
    }
}
