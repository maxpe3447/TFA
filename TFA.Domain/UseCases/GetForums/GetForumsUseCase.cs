using MediatR;

namespace TFA.Forum.Domain.UseCases.GetForums;

internal class GetForumsUseCase : IRequestHandler<GetForumQuery, IEnumerable<Models.Forum>>
{
    private readonly IGetForumsStorage getForumsStorage;

    public GetForumsUseCase(
        IGetForumsStorage getForumsStorage)
    {
        this.getForumsStorage = getForumsStorage;
    }
    public async Task<IEnumerable<Models.Forum>> Handle(GetForumQuery query, CancellationToken cancellationToken)
    {
        //await Task.Delay(2000);
        return await getForumsStorage.GetForums(cancellationToken);
    }
}
