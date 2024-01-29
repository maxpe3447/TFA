using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Models;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain.UseCases.GetTopics;

namespace TFA.Domain.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumDomain(this IServiceCollection services)
    {
        services
            .AddScoped<IGetForumsUseCase, GetForumsUseCase>()
            .AddScoped<ICreateTopicUseCase, CreateTopicUseCase>()
            .AddScoped<IGetTopicsUseCase, GetTopicUseCase>()
            .AddScoped<IIntentionResolver, TopicIntentionalResolver>();

        services
            .AddScoped<IIntentionManager, IntentionManager>()
            .AddScoped<IIdentityProvider, IdentityProvider>();

        services
            .AddValidatorsFromAssemblyContaining<Forum>(includeInternalTypes: true);
        //builder.Services.AddScoped<IValidator<CreateTopicCommand>, CreateTopicCommandValidator>();

        services
            .AddMemoryCache();
        return services;
    }
}
