using FluentAssertions;
using Moq;
using System.Net;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCases.CreateForum;

namespace TFA.Domain.Tests.Authorization;

public class IntentionManagerShould
{
    [Fact]
    public void ReturnFalse_WhenMatchingResolve()
    {
        var resolvers = new IIntentionResolver[]
        {
            new Mock<IIntentionResolver<DomainErrorCode>>().Object,
            new Mock<IIntentionResolver<HttpStatusCode>>().Object
        };
        var sut = new IntentionManager(
            resolvers,
            new Mock<IdentityProvider>().Object);

        sut.IsAllowed(ForumIntention.Create).Should().BeFalse();
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData (false, false)]
    public void ReturnMatchingResolverResult(bool expectedResolverResult, bool expected)
    {
        var resolver = new Mock<IIntentionResolver<ForumIntention>>();
        resolver
            .Setup(r=>r.IsAllowed(It.IsAny<IIdentity>(), It.IsAny<ForumIntention>()))
            .Returns(expectedResolverResult);

        var identityProvider = new Mock<IIdentityProvider>();
        identityProvider
            .Setup(p => p.Current)
            .Returns(new User(Guid.Parse("6e069d03-5e34-4ab5-ae05-0585e02ba5a3"), Guid.Empty));

        var sut = new IntentionManager(
            new IIntentionResolver[]{ resolver.Object },
            identityProvider.Object);

        sut.IsAllowed(ForumIntention.Create).Should().Be(expected);
    }
}
