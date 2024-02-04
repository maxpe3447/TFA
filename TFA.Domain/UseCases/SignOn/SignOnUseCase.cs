using FluentValidation;
using System.Runtime.Serialization;
using TFA.Domain.Authentication;

namespace TFA.Domain.UseCases.SignOn;

internal class SignOnUseCase : ISignOnUseCase
{
    private readonly IValidator<SignOnCommand> validator;
    private readonly IPasswordManager passwordManager;
    private readonly ISignOnStorage signOnStorage;

    public SignOnUseCase(
        IValidator<SignOnCommand> validator,
        IPasswordManager passwordManager,
        ISignOnStorage signOnStorage)
    {
        this.validator = validator;
        this.passwordManager = passwordManager;
        this.signOnStorage = signOnStorage;
    }
    public async Task<IIdentity> Execute(SignOnCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var (salt, hash) = passwordManager.GeneratePasswordParts(command.Password);
        var userId = await signOnStorage.CreateUser(command.Login, salt, hash, cancellationToken);

        return new User(userId);
    }
}
