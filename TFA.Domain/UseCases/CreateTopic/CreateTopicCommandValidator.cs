using FluentValidation;
using TFA.Forum.Domain.Exceptions;
using TFA.Forum.Domain.UseCases.CreateTopic;

namespace TFA.Forum.Domain.UseCases.CreateTopic;

internal class CreateTopicCommandValidator: AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(c => c.ForumId).NotEmpty().WithErrorCode(ValidationErrorCode.Empty);
        RuleFor(c => c.Title)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty)
            .MaximumLength(100).WithErrorCode(ValidationErrorCode.TooLong);
    }
}
