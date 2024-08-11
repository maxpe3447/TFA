using FluentValidation;
using MediatR;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Domain.UseCases.CreateTopic;

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
        var storage = scope.GetStorage<ICreateTopicStorage>();
        var domainEventStorage = scope.GetStorage<IDomainEventStorage>();

        var topic = await storage.CreateTopic(forumId, identityProvider.Current.UserId, title, cancellationToken);
        await domainEventStorage.AddEvent(topic, cancellationToken);
        await scope.Commit(cancellationToken);

        return topic;
    }
}
