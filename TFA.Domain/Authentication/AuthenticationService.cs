using System.Security.Cryptography;
using TFA.Storage.Storages;

using Microsoft.Extensions.Options;
namespace TFA.Domain.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly IAunthenticationStorage storage;
    private readonly ISecurityManager securityManager;
    private readonly Lazy<TripleDES> tripleDES = new(TripleDES.Create);
    private readonly AuthenticationConfiguration configuration;
    public AuthenticationService(
        IAunthenticationStorage aunthenticationStorage,
        ISecurityManager securityManager,
        IOptions<AuthenticationConfiguration> options)
    {
        this.storage = aunthenticationStorage;
        this.securityManager = securityManager;
        configuration = options.Value;
    }
    public async Task<IIdentity> Authenticate(string authToken, CancellationToken cancellationToken)
    {
        using var decryptedStream = new MemoryStream();
        var key = Convert.FromBase64String(configuration.Key);
        var iv = Convert.FromBase64String(configuration.Iv);

        await using (var stream  = new CryptoStream(
            decryptedStream,
            tripleDES.Value.CreateDecryptor(key, iv),
            CryptoStreamMode.Write))
        {
            var encryptedBytes = Convert.FromBase64String(authToken);
            await stream.WriteAsync(encryptedBytes, cancellationToken);
        }

        var userId = new Guid(decryptedStream.ToArray());
        return new User(userId);
    }

    public async Task<(bool success, string authToken)> SignIn(BasicSignInCredentials credentials, CancellationToken cancellationToken)
    {
        var recognaseUser = await storage.FindUser(credentials.Login, cancellationToken);

        if(recognaseUser is null)
        {
            throw new Exception("User not fount");
        }
        
        var success = securityManager.ComparePasswords(credentials.Password,recognaseUser.Salt, recognaseUser.PasswordHash);
        var userIdByte = recognaseUser.UserId.ToByteArray();

        using var encriptedStream = new MemoryStream();
        var iv = Convert.FromBase64String(configuration.Iv);
        var key = Convert.FromBase64String(configuration.Key);


        await using (var stream = new CryptoStream(
            encriptedStream,
            tripleDES.Value.CreateEncryptor(key, iv),
            CryptoStreamMode.Write))
        {
            await stream.WriteAsync(userIdByte, cancellationToken);
        }
            return (success, Convert.ToBase64String(encriptedStream.ToArray()));
    }
}