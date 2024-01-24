using TFA.Domain.Models;
namespace TFA.Domain.UseCases.GetForums
{
    public class GetForumsUseCase : IGetForumsUseCase
    {
        private readonly IGetForumsStorage getForumsStorage;

        public GetForumsUseCase(
            IGetForumsStorage getForumsStorage)
        {
            this.getForumsStorage = getForumsStorage;
        }
        public Task<IEnumerable<Forum>> Execute(CancellationToken cancellationToken)
            => getForumsStorage.GetForums(cancellationToken);
    }
}
