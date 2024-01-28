using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetTopics;

internal class GetTopicUseCase : IGetTopicsUseCase
{
    public Task<(IEnumerable<Topic> resource, int totalCount)> Execute(GetTopicQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
