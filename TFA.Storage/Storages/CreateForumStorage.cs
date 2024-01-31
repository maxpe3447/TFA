using Microsoft.EntityFrameworkCore;
using TFA.Domain.Models;
using TFA.Domain.UseCases.CreateForum;
using TFA.Storage.Entities;

namespace TFA.Storage.Storages;

internal class CreateForumStorage : ICreateForumStorage
{
    private readonly IGuidFactory guidFactory;
    private readonly ForumDbContext forumDbContext;

    public CreateForumStorage(
        IGuidFactory guidFactory,
        ForumDbContext forumDbContext)
    {
        this.guidFactory = guidFactory;
        this.forumDbContext = forumDbContext;
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

        return await forumDbContext.Forums
            .Where(f => f.ForumId == forum.ForumId)
            .Select(f => new Domain.Models.Forum
            {
                Id = f.ForumId,
                Title = f.Title
            })
            .FirstAsync();
    }
}
