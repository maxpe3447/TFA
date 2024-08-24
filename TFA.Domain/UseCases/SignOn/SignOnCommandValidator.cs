using FluentValidation;
using TFA.Forum.Domain.Exceptions;

namespace TFA.Forum.Domain.UseCases.SignOn;

public class SignOnCommandValidator : AbstractValidator<SignOnCommand>
{
    public SignOnCommandValidator()
    {
        RuleFor(c => c.Login)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty)
            .MaximumLength(20).WithErrorCode(ValidationErrorCode.TooLong);
        RuleFor(c => c.Password)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty);
    }
}
