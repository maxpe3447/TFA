using MediatR;
using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Monitoring;

namespace TFA.Forum.Domain.UseCases.SignIn;

public record SignInCommand(string Login, string Password)
    : IRequest<(IIdentity identity, string token)>, IMonitorRequest
{
    private const string CounterName = "user.sign-in";

    void IMonitorRequest.MonitorFailure(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(false));
    }

    void IMonitorRequest.MonitorSuccess(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(true));

    }
}
