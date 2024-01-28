using Microsoft.EntityFrameworkCore;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Storage.Storages
{
    internal class GetForumStorage : IGetForumsStorage
    {
        private readonly ForumDbContext forumDbContext;

        public GetForumStorage(
            ForumDbContext forumDbContext)
        {
            this.forumDbContext = forumDbContext;
        }
        public async Task<IEnumerable<Forum>> GetForums(CancellationToken cancellationToken)
        {
            return await forumDbContext.Forums
                .Select(f => new Forum
                {
                    Id = f.ForumId,
                    Title = f.Title,
                }).ToArrayAsync(cancellationToken);
        }
    }
}
