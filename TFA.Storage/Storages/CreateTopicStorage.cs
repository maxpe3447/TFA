using Microsoft.EntityFrameworkCore;
using TFA.Domain;
using TFA.Domain.Models;
using TFA.Domain.UseCases.CreateTopic;
namespace TFA.Storage.Storages;
public class CreateTopicStorage : ICreateTopicStorage
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
            ForumId= forumId, 
            Title = title,
            UserId = userId,
            TopicId = guidFactory.Create(),
            CreatedAt = momentProvider.Now
        };

        await forumDbContext.Topics.AddAsync(topic, cancellationToken);
        await forumDbContext.SaveChangesAsync(cancellationToken);


    }

    public Task<bool> ForumExists(Guid forumId, CancellationToken cancellationToken)
    {
        forumDbContext.Forums.AnyAsync(f => f.ForumId == forumId, cancellationToken);
    }
}
