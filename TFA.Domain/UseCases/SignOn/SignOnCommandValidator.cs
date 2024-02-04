using FluentValidation;
using TFA.Domain.Exceptions;

namespace TFA.Domain.UseCases.SignOn;

public class SignOnCommandValidator :AbstractValidator<SignOnCommand>
{
    public SignOnCommandValidator()
    {
        RuleFor(c => c.Login)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty);
        RuleFor(c => c.Password)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty);
    }
}
