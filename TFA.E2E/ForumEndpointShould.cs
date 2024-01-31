using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TFA.E2E;

public class ForumEndpointShould:IClassFixture<ForumApiApplicationFactory>
{
    private readonly WebApplicationFactory<Program> factory;

    public ForumEndpointShould(ForumApiApplicationFactory factory)
    {
        this.factory = factory;
    }
    [Fact]
    public async Task ReturnListOfForums()
    {
        using var httpClient = factory.CreateClient();
        using var response = await httpClient.GetAsync("forums");

        response.Invoking(r => r.EnsureSuccessStatusCode()).Should().NotThrow();

        var result = await response.Content.ReadAsStringAsync();

        result.Should().Be("[]");
    }
}
