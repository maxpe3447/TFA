using FluentValidation;
using MediatR;
using TFA.Domain.Authorization;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateForum;

internal class CreateForumUseCase : IRequestHandler<CreateForumCommand, Forum>
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

    public async Task<Forum> Handle(CreateForumCommand command, CancellationToken cancellationToken)
    {

        await validator.ValidateAndThrowAsync(command, cancellationToken);
        intention.ThrowIfForbidden(ForumIntention.Create);

        return await createForumStorage.Create(command.Title, cancellationToken);
    }
}
