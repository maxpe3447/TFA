using MediatR;
using TFA.Domain.Models;
using TFA.Domain.Monitoring;

namespace TFA.Domain.UseCases.CreateTopic;

public record CreateTopicCommand(Guid ForumId, string Title) : IRequest<Topic>, IMonitorRequest
{
    private const string CounterName = "topics.created";

    void IMonitorRequest.MonitorFailure(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(false));
    }

    void IMonitorRequest.MonitorSuccess(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(true));

    }
}
