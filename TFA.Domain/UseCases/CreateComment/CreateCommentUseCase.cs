using MediatR;
using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Authorization;
using TFA.Forum.Domain.DomainEvents;
using TFA.Forum.Domain.Exceptions;
using TFA.Forum.Domain.Models;
using TFA.Forum.Domain.UseCases.CreateTopic;

namespace TFA.Forum.Domain.UseCases.CreateComment;

internal class CreateCommentUseCase(
    IIntentionManager intentionManager,
    IIdentityProvider identityProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCommentCommand, Comment>
{
    public async Task<Comment> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        await using var scope = await unitOfWork.StartScope(cancellationToken);
        var storage = scope.GetStorage<ICreateCommentStorage>();

        var topic = await storage.FindTopic(request.TopicId, cancellationToken);
        if (topic is null)
        {
            throw new TopicNotFoundException(request.TopicId);
        }

        intentionManager.ThrowIfForbidden(TopicIntention.CreateComment, topic);

        var domainEventsStorage = scope.GetStorage<IDomainEventStorage>();
        var comment = await storage.Create(
            request.TopicId, identityProvider.Current.UserId, request.Text, cancellationToken);
        await domainEventsStorage.AddEvent(ForumDomainEvent.CommentCreated(topic, comment), cancellationToken);

        await scope.Commit(cancellationToken);

        return comment;
    }
}
