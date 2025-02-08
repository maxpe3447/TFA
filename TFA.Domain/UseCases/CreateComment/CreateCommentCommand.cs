using MediatR;
using TFA.Forum.Domain.Models;
using TFA.Forum.Domain.Monitoring;

namespace TFA.Forum.Domain.UseCases.CreateComment;

public record CreateCommentCommand(Guid TopicId, string Text) : IRequest<Comment>, IMonitorRequest
{
    private const string CounterName = "comments.created";
    void IMonitorRequest.MonitorFailure(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(true));
    }

    void IMonitorRequest.MonitorSuccess(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(false));
    }
}