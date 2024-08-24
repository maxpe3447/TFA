using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TFA.Forum.Domain;
using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.UseCases;
using TFA.Forum.Domain.UseCases.CreateForum;
using TFA.Forum.Domain.UseCases.CreateTopic;
using TFA.Forum.Domain.UseCases.GetForums;
using TFA.Forum.Domain.UseCases.GetTopics;
using TFA.Forum.Domain.UseCases.SignIn;
using TFA.Forum.Domain.UseCases.SignOn;
using TFA.Forum.Domain.UseCases.SignOut;
using TFA.Forum.Storage;
using TFA.Forum.Storage.Storages;

namespace TFA.Forum.Storage.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumStorage(this IServiceCollection services, string connectionString)
    {
        services
            .AddScoped<IDomainEventStorage, DomainStorage>()
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
            .AddSingleton<IUnitOfWork>(sp => new UnitOfWork(sp));

        services
            .AddMemoryCache();

        services
            .AddAutoMapper(config => config.AddMaps(Assembly.GetAssembly(typeof(ForumDbContext))));
        return services;
    }
}
