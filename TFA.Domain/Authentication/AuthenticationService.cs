using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using TFA.Forum.Domain.Authentication;

namespace TFA.Domain.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationConfiguration configuration;
    private readonly ISymmetricDecryptor _decryptor;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IAuthenticationStorage _authenticationStorage;

    public AuthenticationService(
        ISymmetricDecryptor decryptor,
        IOptions<AuthenticationConfiguration> options,
        ILogger<AuthenticationService> logger,
        IAuthenticationStorage authenticationStorage)
    {
        configuration = options.Value;
        _decryptor = decryptor;
        _logger = logger;
        _authenticationStorage = authenticationStorage;
    }

    public async Task<IIdentity> Authenticate(string authToken, CancellationToken cancellationToken)
    {
        string sessionIdString;
        try
        {
            sessionIdString = await _decryptor.Decrypt(authToken, configuration.Key, cancellationToken);
        }
        catch (CryptographicException ex)
        {
            _logger.LogWarning(
                ex,
                "Cannot decrypt auth token, maybe someone is trying to force it");
            return User.Guest;
        }

        if(!Guid.TryParse(sessionIdString, out var sessionId))
        { 
            return User.Guest;
        }

        var session = await _authenticationStorage.FindSessionId(sessionId, cancellationToken);
        
        if(session is null)
        {
            return User.Guest;
        }

        if(session.ExpiresAt < DateTime.UtcNow)
        {
            return User.Guest;
        }

        //TODO: verify user identity
        return new User(session.UserId, session.Id);
    }
}