using FluentAssertions;
using TFA.Forum.Domain.Authentication;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TFA.Forum.Domain.Tests.Authentication;

public class PasswordManagerShould
{
    private readonly PasswordManager sut = new();
    private static byte[] emptySalt = Enumerable.Repeat((byte)0, 100).ToArray();
    private static byte[] emptyHash = Enumerable.Repeat((byte)0, 32).ToArray();
    [Theory]
    [InlineData("password")]
    [InlineData("qwerty123")]
    public void GenerateMeaningfulSaltAndHash(string password)
    {
        var (salt, hash) = sut.GeneratePasswordParts(password);
        salt.Should().NotBeEmpty().And.HaveCount(100).And.NotBeEquivalentTo(emptySalt);
        hash.Should().HaveCount(32).And.NotBeEquivalentTo(emptyHash);
    }

    [Fact]
    public void ReturnTruPWhenPasswordMatch()
    {
        var password = "qwerty123";
        var (salt, hash) = sut.GeneratePasswordParts(password);
        sut.ComparePasswords(password, salt, hash).Should().BeTrue();
    }

    [Fact]
    public void ReturnFalse_WhenPasswordDoesntMatch()
    {
        var (salt, hash) = sut.GeneratePasswordParts("qwerty123");
        sut.ComparePasswords("p@s$w0rd", salt, hash).Should().BeFalse();
    }
}
