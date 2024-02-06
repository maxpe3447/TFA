using FluentAssertions;
using System.Net.Http.Json;
using TFA.Domain.Authentication;

namespace TFA.E2E;

public class AccountEndpointShould :IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory factory;

    public AccountEndpointShould(ForumApiApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task SignInAfterSignOn()
    {
        using var httpClient = factory.CreateClient();

        using var signOnResponse =  await httpClient.PostAsync(
            "account", JsonContent.Create(new {login = "Test", password = "qwerty"}));

        signOnResponse.IsSuccessStatusCode.Should().BeTrue();
        var createdUser = await signOnResponse.Content.ReadFromJsonAsync<User>();

        using var signInResponse = await httpClient.PostAsync(
            "account/signin", JsonContent.Create(new { login = "Test", password = "qwerty" }));

        signInResponse.IsSuccessStatusCode.Should().BeTrue();
        signInResponse.Headers.Should().ContainKey("TFA-Auth-Token");
        var signedUser = await signInResponse.Content.ReadFromJsonAsync<User>();
        signedUser.Should()
            .NotBeNull().And
            .BeEquivalentTo(createdUser);
    }

}
