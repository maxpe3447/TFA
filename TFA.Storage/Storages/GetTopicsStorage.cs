using Microsoft.EntityFrameworkCore;
using TFA.Forum.Domain.Models;
using TFA.Forum.Domain.UseCases.GetTopics;
using TFA.Forum.Storage;

namespace TFA.Forum.Storage.Storages;

internal class GetTopicsStorage : IGetTopicsStorage
{
    private readonly ForumDbContext forumDbContext;

    public GetTopicsStorage(ForumDbContext forumDbContext)
    {
        this.forumDbContext = forumDbContext;
    }
    public async Task<(IEnumerable<Topic> resources, int totalCount)> GetTopics(Guid forumId, int skip, int take, CancellationToken cancellationToken)
    {
        var query = forumDbContext.Topics.Where(t => t.ForumId == forumId);


        var totalCount = await query.CountAsync(cancellationToken);
        var resources = await query
            .Select(t => new Topic
            {
                Id = t.TopicId,
                ForumId = forumId,
                UserId = t.UserId,
                Title = t.Title,
                CreatedAt = t.CreatedAt,
            })
            .Skip(skip)
            .Take(take)
            .ToArrayAsync(cancellationToken);

        return (resources, totalCount);
    }
}
