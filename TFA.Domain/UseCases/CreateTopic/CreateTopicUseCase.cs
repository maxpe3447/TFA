using TFA.Domain.Exceptions;
using TFA.Domain.Authentication;
using TFA.Domain.Models;
using TFA.Domain.Authorization;
using FluentValidation;

namespace TFA.Domain.UseCases.CreateTopic;

internal class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly IValidator<CreateTopicCommand> validator;
    private readonly IIntentionManager intentionManager;
    private readonly ICreateTopicStorage storage;
    private readonly IIdentityProvider identityProvider;

    public CreateTopicUseCase(
        IValidator<CreateTopicCommand> validator,
        IIntentionManager intentionManager,
        ICreateTopicStorage storage,
        IIdentityProvider identityProvider)
    {
        this.validator = validator;
        this.intentionManager = intentionManager;
        this.storage = storage;
        this.identityProvider = identityProvider;
    }
    public async Task<Topic> Execute(CreateTopicCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var (forumId, title) = command;
        intentionManager.ThrowIfForbidden(TopicIntention.Create);

        var forumExists = await storage.ForumExists(forumId, cancellationToken);
        if (!forumExists)
        {
            throw new ForumNotFoundException(forumId);
        }
        return await storage.CreateTopic(forumId, identityProvider.Current.UserId, title, cancellationToken);
    }
}
