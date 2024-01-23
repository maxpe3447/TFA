using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCases.CreateTopic;

namespace TFA.Domain.Tests
{
    public class CreateTopicUseCaseShould
    {
        private readonly Mock<ICreateTopicStorage> storage;
        private readonly ISetup<ICreateTopicStorage, Task<bool>> forumExistsSetup;
        private readonly ISetup<ICreateTopicStorage, Task<Models.Topic>> createTopicSetup;
        private readonly CreateTopicUseCase sut;
        public CreateTopicUseCaseShould()
        {
            storage = new Mock<ICreateTopicStorage>();
            forumExistsSetup = storage.Setup(s => s.ForumExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
            createTopicSetup = storage.Setup(s => s.CreateTopic(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

            sut = new CreateTopicUseCase(storage.Object);
        }

        [Fact]
        public async void ThrowForumNotFoundException_WhenNoMatchingForum()
        {
            forumExistsSetup.ReturnsAsync(false);

            var forumId = Guid.Parse("8249981e-400e-4111-91c2-6952e1d3fc4a");
            var authorId = Guid.Parse("b01c442e-c3a7-4d34-af8d-3c70ec4dba02");

            await sut.Invoking(s => s.Execute(forumId, "Some Title", authorId, CancellationToken.None))
                .Should().ThrowAsync<ForumNotFoundException>();

            storage.Verify(s => s.ForumExists(forumId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task ReturnNewlyCreatedTopic_WhenMatchingForumExists()
        {
            forumExistsSetup.ReturnsAsync(true);
            var expected = new Models.Topic();
            createTopicSetup.ReturnsAsync(expected);

            var forumId = Guid.Parse("56acc1cc-6bc9-4fa4-8212-3fa433927c8b");
            var userId = Guid.Parse("8c4a7a7b-bb9a-401b-a8f9-c485ce85fc9f");

            var actual = await sut.Execute(forumId, "Hello world", userId, CancellationToken.None);
            actual.Should().Be(expected);

            storage.Verify(s => s.CreateTopic(forumId, userId, "Hello world", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}