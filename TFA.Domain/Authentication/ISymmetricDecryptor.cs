namespace TFA.Domain.Authentication;

internal interface ISymmetricDecryptor
{
    Task<string> Decryptor(string encrypted, byte[] key, CancellationToken token);
}
