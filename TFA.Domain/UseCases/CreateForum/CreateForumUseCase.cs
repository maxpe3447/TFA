using FluentValidation;
using MediatR;
using TFA.Forum.Domain.Authorization;

namespace TFA.Forum.Domain.UseCases.CreateForum;

internal class CreateForumUseCase : IRequestHandler<CreateForumCommand, Models.Forum>
{
    private readonly IValidator<CreateForumCommand> validator;
    private readonly IIntentionManager intention;
    private readonly ICreateForumStorage createForumStorage;

    public CreateForumUseCase(
        IValidator<CreateForumCommand> validator,
        IIntentionManager intention,
        ICreateForumStorage createForumStorage)
    {
        this.validator = validator;
        this.intention = intention;
        this.createForumStorage = createForumStorage;
    }

    public async Task<Models.Forum> Handle(CreateForumCommand command, CancellationToken cancellationToken)
    {
        intention.ThrowIfForbidden(ForumIntention.Create);

        return await createForumStorage.Create(command.Title, cancellationToken);
    }
}
