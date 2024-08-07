using MediatR;
using TFA.Domain.Models;
using TFA.Domain.Monitoring;

namespace TFA.Domain.UseCases.GetForums;

public record GetForumQuery() : IRequest<IEnumerable<Forum>>, IMonitorRequest
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
