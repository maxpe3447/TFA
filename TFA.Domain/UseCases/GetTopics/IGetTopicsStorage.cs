using TFA.Forum.Domain.Models;

namespace TFA.Forum.Domain.UseCases.GetTopics;

public interface IGetTopicsStorage
{
    Task<(IEnumerable<Topic> resources, int totalCount)> GetTopics(
        Guid forumId,
        int skip,
        int take,
        CancellationToken cancellationToken);
}
