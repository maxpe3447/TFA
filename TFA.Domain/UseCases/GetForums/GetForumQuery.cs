using MediatR;
using TFA.Forum.Domain.Monitoring;

namespace TFA.Forum.Domain.UseCases.GetForums;

public record GetForumQuery() : IRequest<IEnumerable<Models.Forum>>, IMonitorRequest
{
    private const string CounterName = "forum.fetch";

    void IMonitorRequest.MonitorFailure(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(false));
    }

    void IMonitorRequest.MonitorSuccess(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(true));

    }
}
