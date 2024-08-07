namespace TFA.Domain.Authentication;

public interface IAuthenticationStorage
{
    Task<Session?> FindSessionId(Guid sessionId, CancellationToken cancellationToken);
}
