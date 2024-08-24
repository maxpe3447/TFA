using MediatR;
using TFA.Forum.Domain.Monitoring;

namespace TFA.Forum.Domain.UseCases.CreateForum;

public record CreateForumCommand(string Title) : IRequest<Models.Forum>, IMonitorRequest
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
