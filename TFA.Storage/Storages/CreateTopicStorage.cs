using Microsoft.EntityFrameworkCore;
using TFA.Forum.Domain.Models;
using TFA.Forum.Domain.UseCases.CreateTopic;

namespace TFA.Forum.Storage.Storages;
internal class CreateTopicStorage : ICreateTopicStorage
{
    private readonly IGuidFactory guidFactory;
    private readonly IMomentProvider momentProvider;
    private readonly ForumDbContext forumDbContext;

    public CreateTopicStorage(
        IGuidFactory guidFactory,
        IMomentProvider momentProvider,
        ForumDbContext forumDbContext)
    {
        this.guidFactory = guidFactory;
        this.momentProvider = momentProvider;
        this.forumDbContext = forumDbContext;
    }
    public async Task<Topic> CreateTopic(Guid forumId, Guid userId, string title, CancellationToken cancellationToken)
    {
        var topic = new Entities.Topic
        {
            ForumId = forumId,
            Title = title,
            UserId = userId,
            TopicId = guidFactory.Create(),
            CreatedAt = momentProvider.Now
        };

        await forumDbContext.Topics.AddAsync(topic, cancellationToken);
        await forumDbContext.SaveChangesAsync(cancellationToken);

        return await forumDbContext.Topics
            .Where(t => t.TopicId == topic.TopicId)
            .Select(t => new Topic
            {
                Id = t.TopicId,
                Title = t.Title,
                UserId = t.UserId,
                ForumId = forumId,
                CreatedAt = t.CreatedAt
            }).FirstAsync(cancellationToken);
    }
}
