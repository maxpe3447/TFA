using TFA.Domain.Authentication;

namespace TFA.Forum.Domain.Authentication;

public interface IAuthenticationStorage
{
    Task<Session?> FindSessionId(Guid sessionId, CancellationToken cancellationToken);
}
