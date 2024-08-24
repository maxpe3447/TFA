using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Forum.Domain.UseCases.CreateForum;

namespace TFA.Forum.Storage.Storages;

internal class CreateForumStorage(
    IGuidFactory guidFactory,
    ForumDbContext forumDbContext,
    IMemoryCache memoryCache,
    IMapper mapper) : ICreateForumStorage
{
    private readonly IGuidFactory guidFactory = guidFactory;
    private readonly ForumDbContext forumDbContext = forumDbContext;
    private readonly IMemoryCache memoryCache = memoryCache;
    private readonly IMapper mapper = mapper;

    public async Task<Domain.Models.Forum> Create(string title, CancellationToken cancellationToken)
    {
        var forum = new Entities.Forum
        {
            ForumId = guidFactory.Create(),
            Title = title
        };

        await forumDbContext.Forums.AddAsync(forum, cancellationToken);
        await forumDbContext.SaveChangesAsync(cancellationToken);

        memoryCache.Remove(nameof(GetForumStorage.GetForums));

        return await forumDbContext.Forums
            .Where(f => f.ForumId == forum.ForumId)
            .ProjectTo<Domain.Models.Forum>(mapper.ConfigurationProvider)
            .FirstAsync();
    }
}
