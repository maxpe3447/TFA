using FluentValidation;
using TFA.Domain.Authorization;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateForum;

internal class CreateForumUseCase : ICreateForumUseCase
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
    public async Task<Forum> Execute(CreateForumCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        intention.ThrowIfForbidden(ForumIntention.Create);

        return await createForumStorage.Create(command.Title, cancellationToken);
    }
}
