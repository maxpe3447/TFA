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
    public async Task<IEnumerable<Forum>> Handle(GetForumQuery query, CancellationToken cancellationToken)
    {
        //await Task.Delay(2000);
        return await getForumsStorage.GetForums(cancellationToken);
    }
}
