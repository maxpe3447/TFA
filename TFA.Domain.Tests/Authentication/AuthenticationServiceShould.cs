using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;
using System.Runtime.CompilerServices;
using TFA.Domain.Authentication;
using TFA.Storage.Entities;
using TFA.Storage.Storages;
namespace TFA.Domain.Tests.Authentication;

public class AuthenticationServiceShould
{
    private readonly AuthenticationService sut;
    private readonly Mock<IAunthenticationStorage> storage;
    private readonly ISetup<IAunthenticationStorage, Task<RecognisedUser?>> findUserSetup;
    private readonly Mock<IOptions<AuthenticationConfiguration>> options;

    public AuthenticationServiceShould()
    {
        storage = new Mock<IAunthenticationStorage>();
        findUserSetup = storage.Setup(s => s.FindUser(It.IsAny<string>(), It.IsAny<CancellationToken>()));

        var securityManager = new Mock<ISecurityManager>();
        securityManager
            .Setup(m => m.ComparePasswords(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        options = new Mock<IOptions<AuthenticationConfiguration>>();
        options
            .Setup(o => o.Value)
            .Returns(new AuthenticationConfiguration
            {
                Key = "QkEeenXpHqgP6t0WwpUetAFvUUZiMb4f",
                Iv = "dtEzMsz2ogg="
            });
        sut = new AuthenticationService(storage.Object, securityManager.Object, options.Object);
    }
    [Fact]
    public async Task ReturnSuccess_WhenUserFound()
    {
        findUserSetup.ReturnsAsync(new RecognisedUser()
        {
            Salt = "k5O4tDqq6lmI4A==",
            PasswordHash = "LLOeMjFDersNpdYrSjRyn2iLWjw==",
            UserId = Guid.Parse("41357dc9-cc38-4dd6-a837-c427f81946b1")
        });

        var (success, authToken) = await sut.SignIn(new BasicSignInCredentials("User", "Password"), CancellationToken.None);
        success.Should().BeTrue();
        authToken.Should().NotBeEmpty();
        
    }
    [Fact]
    public async Task AuthenticateUser_AfterTheySignIn()
    {
        var userId = Guid.Parse("7c98e685-a088-4103-8022-b4ff19b76488");
        findUserSetup.ReturnsAsync(new RecognisedUser()
        {
            UserId = userId
        });

        var (_, authToken) = await sut.SignIn(new BasicSignInCredentials("User", "Password"), CancellationToken.None);

        var identity = await sut.Authenticate(authToken, CancellationToken.None);
        identity.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task SignInUser_WhenPasswordMatches()
    {
        var password = "qwerty";

        var securityManager = new SecurityManager();
        var(salt, hash) = securityManager.GeneratePasswordParts(password);

        findUserSetup.ReturnsAsync(new RecognisedUser()
        {
            UserId = Guid.Parse("082cfc9f-b9e7-4eee-b959-03f633083194"),
            Salt = salt,
            PasswordHash = hash
        });

        var localSut = new AuthenticationService(storage.Object, new SecurityManager(), options.Object);
        var (success, _) = await localSut.SignIn(new BasicSignInCredentials("User", password), CancellationToken.None);
        success.Should().BeTrue();
    }
}
