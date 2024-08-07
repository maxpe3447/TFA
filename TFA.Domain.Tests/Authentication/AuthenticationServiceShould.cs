using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authentication;

namespace TFA.Domain.Tests.Authentication;

public class AuthenticationServiceShould
{
    private readonly AuthenticationService sut;
    private readonly ISetup<ISymmetricDecryptor, Task<string>> setupDecryptor;

    public AuthenticationServiceShould()
    {
        var decryptor = new Mock<ISymmetricDecryptor>();
        setupDecryptor = decryptor.Setup(d => d.Decrypt(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()));

        var options = new Mock<IOptions<AuthenticationConfiguration>>();
        options
            .Setup(c => c.Value)
            .Returns(new AuthenticationConfiguration
            {
                Base64Key = "OZAnTVuMoUGKXiYbyup1mIWsCIn2Q/TIxLnHNHo9zBs="
            });

        var authentication = new Mock<IAuthenticationStorage>();

        sut = new AuthenticationService(decryptor.Object, options.Object, new NullLogger<AuthenticationService>(), authentication.Object);
    }

    [Fact]
    public async Task ExtracctAndReturnIdentityFromToken()
    {
        setupDecryptor.ReturnsAsync("e36a4543-4220-4044-914f-374f6ec0ba68");

        var actual = await sut.Authenticate("some token", CancellationToken.None);

        actual.Should().BeEquivalentTo(new User(Guid.Parse("e36a4543-4220-4044-914f-374f6ec0ba68"), Guid.Empty));
    }
}
