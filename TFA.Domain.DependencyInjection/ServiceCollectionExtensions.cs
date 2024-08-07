using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;
using TFA.Domain.Models;
using TFA.Domain.Monitoring;
using TFA.Domain.UseCases;
using TFA.Domain.UseCases.CreateForum;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain.UseCases.GetTopics;
using TFA.Domain.UseCases.SignIn;
using TFA.Domain.UseCases.SignOn;
using TFA.Domain.UseCases.SignOut;

namespace TFA.Domain.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumDomain(this IServiceCollection services)
    {
        services.AddMediatR(cfg=>cfg
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
            .AddValidatorsFromAssemblyContaining<Forum>(includeInternalTypes: true);
        //builder.Services.AddScoped<IValidator<CreateTopicCommand>, CreateTopicCommandValidator>();

        services
            .AddSingleton<DomainMetrics>();

        return services;
    }
}
