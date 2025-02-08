using FluentValidation;
using TFA.Forum.Domain.Exceptions;

namespace TFA.Forum.Domain.UseCases.CreateComment;

internal class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(c => c.TopicId)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty);
        RuleFor(c => c.Text)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty);
    }
}
