using TFA.Domain.Exceptions;
using TFA.Domain.Authentication;
using TFA.Domain.Models;
using TFA.Domain.Authorization;

namespace TFA.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly IIntentionManager intentionManager;
    private readonly ICreateTopicStorage storage;
    private readonly IIdentityProvider identityProvider;

    public CreateTopicUseCase(
        IIntentionManager intentionManager,
        ICreateTopicStorage storage,
        IIdentityProvider identityProvider)
    {
        this.intentionManager = intentionManager;
        this.storage = storage;
        this.identityProvider = identityProvider;
    }
    public async Task<Topic> Execute(Guid forumId, string title, CancellationToken cancellationToken)
    {
        intentionManager.ThrowIfForbidden(TopicIntention.Create);

        var forumExists = await storage.ForumExists(forumId, cancellationToken);
        if (!forumExists)
        {
            throw new ForumNotFoundException(forumId);
        }
        return await storage.CreateTopic(forumId, identityProvider.Current.UserId, title, cancellationToken);
    }
}
