using FluentAssertions;
using Moq;
using TFA.Domain.Authentication;
using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.UseCases.CreateTopic;

namespace TFA.Forum.Domain.Tests.Authorization;

public class TopicResolverShould
{
    private readonly TopicIntentionalResolver sut = new();
    [Fact]
    public void ReturnFalse_WhenIntentionNotInEnum()
    {
        var intention = (TopicIntention)(-1);
        sut.IsAllowed(new Mock<IIdentity>().Object, intention)
            .Should().BeFalse();
    }

    [Fact]
    public void ReturnFalse_WhenCheckingTopicCreateIntention_AndUserIsGuest()
    {
        sut.IsAllowed(User.Guest, TopicIntention.Create).Should().BeFalse();
    }
    [Fact]
    public void ReturnTrue_WhenCheckingTopicCreateIntention_AndUserIsAuthenticated()
    {
        sut.IsAllowed(
            new User(Guid.Parse("20811a77-5b86-4e99-a0d1-6a4819b78721"), Guid.Empty),
            TopicIntention.Create)
            .Should().BeTrue();
    }
}
