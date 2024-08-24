using TFA.Forum.Domain.Models;

namespace TFA.Forum.Domain.UseCases.CreateTopic;

public interface ICreateTopicStorage : IStorage
{
    Task<Topic> CreateTopic(Guid forumId, Guid userId, string title, CancellationToken cancellationToken);
}
