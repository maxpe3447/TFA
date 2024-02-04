using Microsoft.Extensions.Options;

namespace TFA.Domain.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationConfiguration configuration;
    private readonly ISymmetricDecryptor decryptor;

    public AuthenticationService(
        ISymmetricDecryptor decryptor,
        IOptions<AuthenticationConfiguration> options)
    {
        configuration = options.Value;
        this.decryptor = decryptor;
    }
    public async Task<IIdentity> Authenticate(string authToken, CancellationToken cancellationToken)
    {
        var UserIdString = await decryptor.Decryptor(authToken,configuration.Key, cancellationToken);
        //TODO: verify user identity
        return new User(Guid.Parse(UserIdString));
    }

}