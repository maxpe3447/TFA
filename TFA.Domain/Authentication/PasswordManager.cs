using System.Security.Cryptography;
using System.Text;

namespace TFA.Forum.Domain.Authentication;

public class PasswordManager : IPasswordManager
{
    private const int SaltLength = 100;
    private readonly Lazy<SHA256> sha256 = new(SHA256.Create);
    public bool ComparePasswords(string password, byte[] salt, byte[] hash)
    {
        return ComputeHash(password, salt).SequenceEqual(hash);
    }

    public (byte[] Salt, byte[] Hash) GeneratePasswordParts(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltLength);

        var hash = ComputeHash(password, salt);
        return (salt, hash.ToArray());
    }

    private ReadOnlySpan<byte> ComputeHash(string palinText, byte[] salt)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(palinText);
        var buffer = new byte[plainTextBytes.Length + salt.Length];
        Array.Copy(plainTextBytes, buffer, plainTextBytes.Length);
        Array.Copy(salt, 0, buffer, plainTextBytes.Length, salt.Length);
        lock (sha256)
        {
            return sha256.Value.ComputeHash(buffer);
        }
    }
}
