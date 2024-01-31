using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Domain.Models;
using TFA.Domain.UseCases.CreateForum;
using TFA.Storage.Entities;

namespace TFA.Storage.Storages;

internal class CreateForumStorage : ICreateForumStorage
{
    private readonly IGuidFactory guidFactory;
    private readonly ForumDbContext forumDbContext;
    private readonly IMemoryCache memoryCache;
    private readonly IMapper mapper;

    public CreateForumStorage(
        IGuidFactory guidFactory,
        ForumDbContext forumDbContext,
        IMemoryCache memoryCache,
        IMapper mapper)
    {
        this.guidFactory = guidFactory;
        this.forumDbContext = forumDbContext;
        this.memoryCache = memoryCache;
        this.mapper = mapper;
    }
    public async Task<Domain.Models.Forum> Create(string title, CancellationToken cancellationToken)
    {
        var forum = new Entities.Forum
        {
            ForumId = guidFactory.Create(),
            Title = title
        };

        await forumDbContext.Forums.AddAsync(forum);
        await forumDbContext.SaveChangesAsync();

        memoryCache.Remove(nameof(GetForumStorage.GetForums));

        return await forumDbContext.Forums
            .Where(f => f.ForumId == forum.ForumId)
            .ProjectTo<Domain.Models.Forum>(mapper.ConfigurationProvider)
            .FirstAsync();
    }
}
