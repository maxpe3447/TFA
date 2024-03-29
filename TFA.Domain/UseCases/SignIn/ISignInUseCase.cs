using FluentValidation;
using Microsoft.Extensions.Options;
using TFA.Domain.Authentication;

namespace TFA.Domain.UseCases.SignIn;

public interface ISignInUseCase
{
    Task<(IIdentity identity, string token)> Execute(SignInCommand command, CancellationToken cancellationToken);
}
internal class SignInUseCase : ISignInUseCase
{
    private readonly IValidator<SignInCommand> validator;
    private readonly ISignInStorage storage;
    private readonly IPasswordManager passwordManager;
    private readonly ISymmetricEncryptor symmetricEncryptor;
    private readonly AuthenticationConfiguration configuration;

    public SignInUseCase(
        IValidator<SignInCommand> validator,
        ISignInStorage storage,
        IPasswordManager passwordManager, 
        ISymmetricEncryptor symmetricEncryptor,
        IOptions<AuthenticationConfiguration> options) 
    {
        this.validator = validator;
        this.storage = storage;
        this.passwordManager = passwordManager;
        this.symmetricEncryptor = symmetricEncryptor;
        configuration = options.Value;
    }

    public async Task<(IIdentity identity, string token)> Execute(
        SignInCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var recognisedUser = await storage.FindUser(command.Login, cancellationToken);
        if(recognisedUser is null) 
        {
            throw new Exception();
        }
        var passwordMatches = passwordManager.ComparePasswords(
            command.Password, recognisedUser.Salt, recognisedUser.PasswordHash);
        if (!passwordMatches)
        {
            throw new Exception();
        }

        var token = await symmetricEncryptor.Encrypt(
            recognisedUser.UserId.ToString(), configuration.Key, cancellationToken);

        return (new User(recognisedUser.UserId), token);
    }
}
