using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Storage;
using TFA.Storage.Entities;

namespace TFA.Domain.Tests
{
    public class CreateTopicUseCaseShould
    {
        private readonly Moq.Language.Flow.ISetup<IMomentProvider, DateTimeOffset> getNowSetup;
        private readonly Moq.Language.Flow.ISetup<IGuidFactory, Guid> createIdSetup;
        private readonly CreateTopicUseCase sut;
        private readonly ForumDbContext dbContext;
        public CreateTopicUseCaseShould()
        {
            var builder = new DbContextOptionsBuilder<ForumDbContext>()
                .UseInMemoryDatabase(nameof(CreateTopicUseCaseShould));
            dbContext = new ForumDbContext(builder.Options);

            var guidFactory = new Mock<IGuidFactory>();
            createIdSetup = guidFactory.Setup(f => f.Create());

            var momentProvider = new Mock<IMomentProvider>();
            getNowSetup = momentProvider.Setup(p => p.Now);

            sut = new CreateTopicUseCase(dbContext, guidFactory.Object, momentProvider.Object);
        }

        [Fact]
        public async void ThrowForumNotFoundException_WhenNoMatchingForum()
        {

            await dbContext.Forums.AddAsync(new Storage.Entities.Forum
            {
                ForumId = Guid.Parse("cb121411-3cdb-4726-9fcd-efa4aaeefc84"),
                Title = "Hello world"
            });
            await dbContext.SaveChangesAsync();

            var forumId = Guid.Parse("8249981e-400e-4111-91c2-6952e1d3fc4a");
            var authorId = Guid.Parse("b01c442e-c3a7-4d34-af8d-3c70ec4dba02");

            await sut.Invoking(s => s.Execute(forumId, "Some Title", authorId, CancellationToken.None))
                .Should().ThrowAsync<ForumNotFoundException>();
        }

        [Fact]
        public async Task ReturnNewlyCreatedTopic()
        {
            var forumId = Guid.Parse("56acc1cc-6bc9-4fa4-8212-3fa433927c8b");
            var userId = Guid.Parse("8c4a7a7b-bb9a-401b-a8f9-c485ce85fc9f");

            await dbContext.Forums.AddAsync(new Storage.Entities.Forum
            {
                ForumId = forumId,
                Title = "Existing Forum"
            });

            await dbContext.Users.AddAsync(new User
            {
                UserId = userId,
                Login = "Aleks"
            });
            await dbContext.SaveChangesAsync();
            
            createIdSetup.Returns(Guid.Parse("e587b04b-c475-4f5c-9283-de594b6c091b"));
            getNowSetup.Returns(new DateTimeOffset(2024, 1, 22, 22, 50, 0, TimeSpan.FromHours(2)));

            var actual = await sut.Execute(forumId, "Hello world", userId, CancellationToken.None);

            //var allTopics = await dbContext.Topics.ToArrayAsync();
            actual.Should().BeEquivalentTo(new Models.Topic
            {
                Id = Guid.Parse("e587b04b-c475-4f5c-9283-de594b6c091b"),
                Author = "Aleks",
                Title = "Hello world",
                CreatedAt = new DateTimeOffset(2024, 1, 22, 22, 50, 0, TimeSpan.FromHours(2))
            });
        }
    }
}