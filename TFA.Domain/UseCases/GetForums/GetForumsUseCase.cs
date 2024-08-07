using MediatR;
using TFA.Domain.Models;
using TFA.Domain.Monitoring;
namespace TFA.Domain.UseCases.GetForums;

internal class GetForumsUseCase : IRequestHandler<GetForumQuery, IEnumerable<Forum>>
{
    private readonly IGetForumsStorage getForumsStorage;

    public GetForumsUseCase(
        IGetForumsStorage getForumsStorage)
    {
        this.getForumsStorage = getForumsStorage;
    }
    public Task<IEnumerable<Forum>> Handle(GetForumQuery query, CancellationToken cancellationToken)
    {
        return getForumsStorage.GetForums(cancellationToken);
    }
}
