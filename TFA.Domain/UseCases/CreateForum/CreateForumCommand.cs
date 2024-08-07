using MediatR;
using TFA.Domain.Models;
using TFA.Domain.Monitoring;

namespace TFA.Domain.UseCases.CreateForum;

public record CreateForumCommand(string Title) : IRequest<Forum>, IMonitorRequest
{
    private const string CounterName = "forum.created";

    void IMonitorRequest.MonitorFailure(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(false));
    }

    void IMonitorRequest.MonitorSuccess(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(true));

    }
}
