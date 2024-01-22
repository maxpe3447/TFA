using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.Runtime.CompilerServices;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Storage;

namespace TFA.Domain.Tests
{
    public class CreateTopicUseCaseShould
    {
        private readonly CreateTopicUseCase sut;
        private readonly ForumDbContext dbContext;
        public CreateTopicUseCaseShould()
        {
            var builder = new DbContextOptionsBuilder<ForumDbContext>()
                .UseInMemoryDatabase(nameof(CreateTopicUseCaseShould));

            dbContext = new ForumDbContext(builder.Options);
            sut = new CreateTopicUseCase(dbContext);
        }

        [Fact]
        public async void ThrowForumNotFoundException_WhenNoMatchingForum()
        {

            await dbContext.Forums.AddAsync(new Storage.Entities.Forum
            {
                ForumId = Guid.NewGuid(),
                Title = "Basic Forum"
            });
            await dbContext.SaveChangesAsync();

            var forumId = Guid.Parse("8249981e-400e-4111-91c2-6952e1d3fc4a");
            var authorId = Guid.Parse("b01c442e-c3a7-4d34-af8d-3c70ec4dba02");

            sut.Invoking(s => s.Execute(forumId, "Some Title", authorId, CancellationToken.None))
                .Should().ThrowAsync<ForumNotFoundException>();
        }
    }
}