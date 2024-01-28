using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetTopics;

public interface IGetTopicsUseCase
{
    Task<(IEnumerable<Topic> resource, int totalCount)> Execute (GetTopicsQuery query, CancellationToken cancellationToken);
}
