using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TFA.Domain.Authentication;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TFA.E2E;

public class AccountEndpointShould :IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory factory;
    private readonly ITestOutputHelper testOutputHelper;

    public AccountEndpointShould(ForumApiApplicationFactory factory,
                                 ITestOutputHelper testOutputHelper)
    {
        this.factory = factory;
        this.testOutputHelper = testOutputHelper;
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
        //signInResponse.Headers.c.Should().ContainKey("TFA-Auth-Token");

        //testOutputHelper.WriteLine("Test");
        //testOutputHelper.WriteLine(string.Join(Environment.NewLine, 
        //    signInResponse.Headers.Select(h=>$"{h.Key} = {string.Join(" ,", h.Value)}")));
        var signedUser = await signInResponse.Content.ReadFromJsonAsync<User>();
        signedUser!.UserId.Should().Be(createdUser!.UserId);
        //signedUser.Should()
        //    .NotBeNull().And.As<User>().Should()
        //    .Be(createdUser.UserId);


        //HttpRequestMessage request = new(HttpMethod.Post, "forums")
        //{
        //     Content = JsonContent.Create(new { title = "test Title" })
        //};
        //request.Headers.Add("TFA-Auth-Token", signInResponse.Headers.GetValues("TFA-Auth-Token"));
        //var createdForumResponse =  await httpClient.SendAsync(request);
        var createdForumResponse = await httpClient.PostAsync(
            "forums", JsonContent.Create(new { title = "test Title" }));
        createdForumResponse.IsSuccessStatusCode.Should().BeTrue();
    }

}
