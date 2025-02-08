using FluentValidation;
using MediatR;
using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Authorization;
using TFA.Forum.Domain.Models;
using TFA.Forum.Domain.UseCases.GetForums;

namespace TFA.Forum.Domain.UseCases.CreateTopic;

internal class CreateTopicUseCase : IRequestHandler<CreateTopicCommand, Topic>
{
    private readonly IValidator<CreateTopicCommand> validator;
    private readonly IIntentionManager intentionManager;
    private readonly IUnitOfWork unitOfWork;
    private readonly IGetForumsStorage getForumsStorage;
    private readonly IIdentityProvider identityProvider;

    public CreateTopicUseCase(
        IValidator<CreateTopicCommand> validator,
        IIntentionManager intentionManager,
        IUnitOfWork unitOfWork,
        IGetForumsStorage getForumsStorage,
        IIdentityProvider identityProvider,
        IDomainEventStorage domainEventStorage)
    {
        this.validator = validator;
        this.intentionManager = intentionManager;
        this.unitOfWork = unitOfWork;
        this.getForumsStorage = getForumsStorage;
        this.identityProvider = identityProvider;
    }

    public async Task<Topic> Handle(CreateTopicCommand command, CancellationToken cancellationToken)
    {
        var (forumId, title) = command;
        intentionManager.ThrowIfForbidden(TopicIntention.Create);

        await getForumsStorage.ThrowIfForumNotFound(forumId, cancellationToken);

        await using var scope = await unitOfWork.StartScope(cancellationToken);
        var topic = await scope
            .GetStorage<ICreateTopicStorage>()
            .CreateTopic(forumId, identityProvider.Current.UserId, title, cancellationToken);

        await scope
            .GetStorage<IDomainEventStorage>()
            .AddEvent(DomainEvents.ForumDomainEvent.TopicCreated(topic), cancellationToken);
        
        await scope.Commit(cancellationToken);

        return topic;
    }
}
