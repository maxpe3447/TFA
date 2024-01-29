using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Storage.Storages;

internal class GetForumStorage : IGetForumsStorage
{
    private readonly ForumDbContext forumDbContext;
    private readonly IMemoryCache memoryCache;

    public GetForumStorage(
        ForumDbContext forumDbContext,
        IMemoryCache memoryCache)
    {
        this.forumDbContext = forumDbContext;
        this.memoryCache = memoryCache;
    }
    public async Task<IEnumerable<Forum>> GetForums(CancellationToken cancellationToken)
    {
        return await memoryCache.GetOrCreateAsync(
            nameof(GetForums), 
                entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
            return forumDbContext.Forums
            .Select(f => new Forum
            {
                Id = f.ForumId,
                Title = f.Title,
            }).ToArrayAsync(cancellationToken);
        });
    }
}
