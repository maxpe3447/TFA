using FluentValidation;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Domain.UseCases.CreateTopic;

internal class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly IValidator<CreateTopicCommand> validator;
    private readonly IIntentionManager intentionManager;
    private readonly ICreateTopicStorage storage;
    private readonly IGetForumsStorage getForumsStorage;
    private readonly IIdentityProvider identityProvider;

    public CreateTopicUseCase(
        IValidator<CreateTopicCommand> validator,
        IIntentionManager intentionManager,
        ICreateTopicStorage storage,
        IGetForumsStorage getForumsStorage,
        IIdentityProvider identityProvider)
    {
        this.validator = validator;
        this.intentionManager = intentionManager;
        this.storage = storage;
        this.getForumsStorage = getForumsStorage;
        this.identityProvider = identityProvider;
    }
    public async Task<Topic> Execute(CreateTopicCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var (forumId, title) = command;
        intentionManager.ThrowIfForbidden(TopicIntention.Create);

        await getForumsStorage.ThrowIfForumNotFound(forumId, cancellationToken);

        return await storage.CreateTopic(forumId, identityProvider.Current.UserId, title, cancellationToken);
    }
}
