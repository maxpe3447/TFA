using TFA.Domain.UseCases.CreateTopic;

namespace TFA.Domain.Authentication;

public interface IAuthenticationService
{
    Task<(bool success, string authToken)> SignIn(BasicSignInCredentials credentials, CancellationToken cancellationToken);
    Task<IIdentity> Authenticate(string authToken, CancellationToken cancellationToken);
}
