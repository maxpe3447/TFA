using FluentAssertions;
using System.Security.Cryptography;
using TFA.Forum.Domain.Authentication;
using Xunit.Abstractions;

namespace TFA.Forum.Domain.Tests.Authentication;

public class AesSymmetricEncryptorDecryptorShould
{
    private readonly AesSymmetricEncryptorDecryptor sut = new();
    private readonly ITestOutputHelper testOutputHelper;

    public AesSymmetricEncryptorDecryptorShould(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }
    [Fact]
    public async Task ReturnMeaningfulEncryptedString()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var actual = await sut.Encrypt("Hello world", key, CancellationToken.None);

        actual.Should().NotBeEmpty();
    }
    [Fact]
    public async Task DecryptEncryptedString_WhenKeyIsSame()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var encrypted = await sut.Encrypt("Hello world!", key, CancellationToken.None);
        var decrypted = await sut.Decrypt(encrypted, key, CancellationToken.None);

        decrypted.Should().Be("Hello world!");
    }

    [Fact]
    public async Task ThrowException_WhenDecryptingWithDifferentKey()
    {
        var encrypted = await sut.Encrypt("Hello world!", RandomNumberGenerator.GetBytes(32), CancellationToken.None);
        await sut.Invoking(s => s.Decrypt(encrypted, RandomNumberGenerator.GetBytes(32), CancellationToken.None))
            .Should().ThrowAsync<CryptographicException>();

    }

    [Fact]
    public void GiveMeBase64Key()
    {
        testOutputHelper.WriteLine(Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)));

    }
}
