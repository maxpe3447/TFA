using TFA.Domain.UseCases.CreateTopic;

namespace TFA.Domain.Authentication;

public interface IAuthenticationService
{
    Task<IIdentity> Authenticate(string authToken, CancellationToken cancellationToken);
}
