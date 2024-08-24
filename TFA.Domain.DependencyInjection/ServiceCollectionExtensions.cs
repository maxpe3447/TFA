using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TFA.Domain.Authentication;
using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Authorization;
using TFA.Forum.Domain.Exceptions;
using TFA.Forum.Domain.Monitoring;
using TFA.Forum.Domain.UseCases;
using TFA.Forum.Domain.UseCases.CreateForum;
using TFA.Forum.Domain.UseCases.CreateTopic;

namespace TFA.Forum.Domain.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumDomain(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg
            .AddOpenBehavior(typeof(MonitoringPipelineBehavior<,>))
            .AddOpenBehavior(typeof(ValidationPipelineBehavior<,>))
            .RegisterServicesFromAssemblies(typeof(DomainErrorCode).Assembly));

        services
            .AddScoped<IIntentionResolver, ForumIntentionResolver>()
            .AddScoped<IIntentionResolver, TopicIntentionalResolver>();

        services
            .AddScoped<IIntentionManager, IntentionManager>()
            .AddScoped<IIdentityProvider, IdentityProvider>()
            .AddScoped<IPasswordManager, PasswordManager>()
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<ISymmetricDecryptor, AesSymmetricEncryptorDecryptor>()
            .AddScoped<ISymmetricEncryptor, AesSymmetricEncryptorDecryptor>();

        services
            .AddValidatorsFromAssemblyContaining<Models.Forum>(includeInternalTypes: true);
        //builder.Services.AddScoped<IValidator<CreateTopicCommand>, CreateTopicCommandValidator>();

        services
            .AddSingleton<DomainMetrics>();

        return services;
    }
}
