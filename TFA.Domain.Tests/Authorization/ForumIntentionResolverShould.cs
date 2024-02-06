using FluentAssertions;
using Moq;
using TFA.Domain.Authentication;
using TFA.Domain.UseCases.CreateForum;

namespace TFA.Domain.Tests.Authorization;

public class ForumIntentionResolverShould
{
    private readonly ForumIntentionResolver sut = new();
    [Fact]
    public void ReturnFalse_WhenIntentionNotInEnum()
    {
        var intention = (ForumIntention)(-1);
        sut.IsAllowed(new Mock<IIdentity>().Object, intention).Should().BeFalse();
    }

    [Fact]
    public void ReturnFalse_WhenCheckingForumCreateIntention_AndUserIsGuest()
    {
        sut.IsAllowed(User.Guest, ForumIntention.Create).Should().BeFalse();
    }
    [Fact]
    public void ReturnTrue_WhenCheckingForumCreateIntention_AndUserIsAuthenticated()
    {
        sut.IsAllowed(
            new User(Guid.Parse("20811a77-5b86-4e99-a0d1-6a4819b78721")),
            ForumIntention.Create)
            .Should().BeTrue();
    }
}
