
using FluentAssertions;
using System.Net.Http.Json;

namespace TFA.Forum.E2E;

public class TopicEndpointsShould(ForumApiApplicationFactory factory)
    : IClassFixture<ForumApiApplicationFactory>, IAsyncLifetime
{
    private readonly Guid forumId = Guid.Parse("027efe19-9659-418a-a872-56c4df2d8da8");
    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ReturnForbidden_WhenNotAuthenticated()
    {
        using var httpClient = factory.CreateClient();

        using var forumCreateResponse = await httpClient.PostAsync("forums", JsonContent.Create(new { title = "Test forum" }));
        forumCreateResponse.EnsureSuccessStatusCode();

        var createdForum = await forumCreateResponse.Content.ReadFromJsonAsync<Forum>();
        createdForum.Should().NotBeNull();


        var response = await httpClient.PostAsync($"forums/{createdForum!.Id}/topics",
            JsonContent.Create(new { title = "Hello world" }));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
}
