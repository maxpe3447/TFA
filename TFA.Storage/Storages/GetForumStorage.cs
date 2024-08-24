using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Forum.Domain.UseCases.GetForums;

namespace TFA.Forum.Storage.Storages;

internal class GetForumStorage : IGetForumsStorage
{
    private readonly ForumDbContext forumDbContext;
    private readonly IMemoryCache memoryCache;
    private readonly IMapper mapper;

    public GetForumStorage(
        ForumDbContext forumDbContext,
        IMemoryCache memoryCache,
        IMapper mapper)
    {
        this.forumDbContext = forumDbContext;
        this.memoryCache = memoryCache;
        this.mapper = mapper;
    }
    public async Task<IEnumerable<Domain.Models.Forum>> GetForums(CancellationToken cancellationToken)
    {
        return await memoryCache.GetOrCreateAsync(
            nameof(GetForums),
                entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
            return forumDbContext.Forums
            .ProjectTo<Domain.Models.Forum>(mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);
        });
    }
}
