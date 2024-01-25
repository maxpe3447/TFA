using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Exceptions;
using TFA.Domain.Authentication;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.Authorization;
using FluentValidation;
using FluentValidation.Results;

namespace TFA.Domain.Tests
{
    public class CreateTopicUseCaseShould
    {
        private readonly Mock<ICreateTopicStorage> storage;
        private readonly ISetup<ICreateTopicStorage, Task<bool>> forumExistsSetup;
        private readonly ISetup<ICreateTopicStorage, Task<Models.Topic>> createTopicSetup;
        private readonly ISetup<IIdentity, Guid> getCurrentUserIdSetup;
        private readonly Mock<IIntentionManager> intentionalManager;
        private readonly ISetup<IIntentionManager, bool> intentionIsAllowedSetup;
        private readonly CreateTopicUseCase sut;
        public CreateTopicUseCaseShould()
        {
            storage = new Mock<ICreateTopicStorage>();
            forumExistsSetup = storage.Setup(s => s.ForumExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
            createTopicSetup = storage.Setup(s => s.CreateTopic(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

            var identity = new Mock<IIdentity>();
            var identityProvider = new Mock<IIdentityProvider>();
            identityProvider.Setup(s => s.Current).Returns(identity.Object);
            getCurrentUserIdSetup =  identity.Setup(s => s.UserId);

            intentionalManager = new Mock<IIntentionManager>();
            intentionIsAllowedSetup = intentionalManager.Setup(s => s.IsAllowed(It.IsAny<TopicIntention>()));

            var validator = new Mock<IValidator<CreateTopicCommand>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult);

            sut = new CreateTopicUseCase(validator.Object, intentionalManager.Object, storage.Object, identityProvider.Object);
        }

        [Fact]
        public async Task ThrowIntentionManagerException_WhenTopicCreationIsNotAllowed()
        {
            var forumId = Guid.Parse("86d52a9d-1a3b-4976-8901-9b9936c4e6ec");

            intentionIsAllowedSetup.Returns(false);
            await sut.Invoking(s=>s.Execute(new(forumId, "Whatever"), CancellationToken.None))
                .Should().ThrowAsync<IntentionManagerException>();

            intentionalManager.Verify(s=>s.IsAllowed(TopicIntention.Create));
        }


        [Fact]
        public async void ThrowForumNotFoundException_WhenNoMatchingForum()
        {
            var forumId = Guid.Parse("8249981e-400e-4111-91c2-6952e1d3fc4a");

            intentionIsAllowedSetup.Returns(true);
            forumExistsSetup.ReturnsAsync(false);

            await sut.Invoking(s => s.Execute(new(forumId, "Some Title"), CancellationToken.None))
                .Should().ThrowAsync<ForumNotFoundException>();

            storage.Verify(s => s.ForumExists(forumId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task ReturnNewlyCreatedTopic_WhenMatchingForumExists()
        {
            var forumId = Guid.Parse("56acc1cc-6bc9-4fa4-8212-3fa433927c8b");
            var userId = Guid.Parse("8c4a7a7b-bb9a-401b-a8f9-c485ce85fc9f");

            intentionIsAllowedSetup.Returns(true);
            forumExistsSetup.ReturnsAsync(true);
            getCurrentUserIdSetup.Returns(userId);
            var expected = new Models.Topic();
            createTopicSetup.ReturnsAsync(expected);

            var actual = await sut.Execute(new(forumId, "Hello world"), CancellationToken.None);
            actual.Should().Be(expected);

            storage.Verify(s => s.CreateTopic(forumId, userId, "Hello world", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}