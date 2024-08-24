using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Authorization;
using TFA.Forum.Domain.Exceptions;
using TFA.Forum.Domain.Models;
using TFA.Forum.Domain.UseCases;
using TFA.Forum.Domain.UseCases.CreateTopic;
using TFA.Forum.Domain.UseCases.GetForums;

namespace TFA.Forum.Domain.Tests.CreateTopic;

public class CreateTopicUseCaseShould
{
    private readonly Mock<ICreateTopicStorage> storage;
    private readonly ISetup<ICreateTopicStorage, Task<Topic>> createTopicSetup;
    private readonly Mock<IGetForumsStorage> getForumsStorage;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Models.Forum>>> getForumsSetUp;
    private readonly ISetup<IIdentity, Guid> getCurrentUserIdSetup;
    private readonly Mock<IIntentionManager> intentionalManager;
    private readonly ISetup<IIntentionManager, bool> intentionIsAllowedSetup;
    private readonly CreateTopicUseCase sut;
    public CreateTopicUseCaseShould()
    {
        storage = new Mock<ICreateTopicStorage>();
        createTopicSetup = storage.Setup(s => s.CreateTopic(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

        getForumsStorage = new Mock<IGetForumsStorage>();
        getForumsSetUp = getForumsStorage.Setup(s => s.GetForums(It.IsAny<CancellationToken>()));

        var identity = new Mock<IIdentity>();
        var identityProvider = new Mock<IIdentityProvider>();
        identityProvider.Setup(s => s.Current).Returns(identity.Object);
        getCurrentUserIdSetup = identity.Setup(s => s.UserId);

        intentionalManager = new Mock<IIntentionManager>();
        intentionIsAllowedSetup = intentionalManager.Setup(s => s.IsAllowed(It.IsAny<TopicIntention>()));

        var validator = new Mock<IValidator<CreateTopicCommand>>();
        validator.Setup(v => v.ValidateAsync(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var unitOfWork = new Mock<IUnitOfWork>();
        var eventStorage = new Mock<IDomainEventStorage>();
        sut = new(validator.Object, intentionalManager.Object, unitOfWork.Object, getForumsStorage.Object, identityProvider.Object, eventStorage.Object);
    }

    [Fact]
    public async Task ThrowIntentionManagerException_WhenTopicCreationIsNotAllowed()
    {
        var forumId = Guid.Parse("86d52a9d-1a3b-4976-8901-9b9936c4e6ec");

        intentionIsAllowedSetup.Returns(false);
        await sut.Invoking(s => s.Handle(new(forumId, "Whatever"), CancellationToken.None))
            .Should().ThrowAsync<IntentionManagerException>();

        intentionalManager.Verify(s => s.IsAllowed(TopicIntention.Create));
    }


    [Fact]
    public async void ThrowForumNotFoundException_WhenNoMatchingForum()
    {
        var forumId = Guid.Parse("8249981e-400e-4111-91c2-6952e1d3fc4a");

        intentionIsAllowedSetup.Returns(true);
        getForumsSetUp.ReturnsAsync(Array.Empty<Models.Forum>());

        (await sut.Invoking(s => s.Handle(new(forumId, "Some Title"), CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>())
            .Which.DomainErrorCode.Should().Be(DomainErrorCode.Gone);
    }

    [Fact]
    public async Task ReturnNewlyCreatedTopic_WhenMatchingForumExists()
    {
        var forumId = Guid.Parse("56acc1cc-6bc9-4fa4-8212-3fa433927c8b");
        var userId = Guid.Parse("8c4a7a7b-bb9a-401b-a8f9-c485ce85fc9f");

        intentionIsAllowedSetup.Returns(true);
        getForumsSetUp.ReturnsAsync(new Models.Forum[] { new Models.Forum { Id = forumId } });
        getCurrentUserIdSetup.Returns(userId);
        var expected = new Topic();
        createTopicSetup.ReturnsAsync(expected);

        var actual = await sut.Handle(new(forumId, "Hello world"), CancellationToken.None);
        actual.Should().Be(expected);

        storage.Verify(s => s.CreateTopic(forumId, userId, "Hello world", It.IsAny<CancellationToken>()), Times.Once);
    }
}