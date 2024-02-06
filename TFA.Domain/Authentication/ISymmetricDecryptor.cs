namespace TFA.Domain.Authentication;

internal interface ISymmetricDecryptor
{
    Task<string> Decrypt(string encrypted, byte[] key, CancellationToken token);
}
