using MediatR;
using TFA.Domain.Models;
using TFA.Domain.Monitoring;

namespace TFA.Domain.UseCases.GetTopics;

public record GetTopicsQuery(Guid ForumId, int Skip, int Take) 
    : IRequest<(IEnumerable<Topic> resource, int totalCount)>, IMonitorRequest
{
    private const string CounterName = "topics.fetched";

    void IMonitorRequest.MonitorFailure(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(false));
    }

    void IMonitorRequest.MonitorSuccess(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(true));

    }
}
