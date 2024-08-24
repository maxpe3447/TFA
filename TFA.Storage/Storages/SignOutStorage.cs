using TFA.Forum.Domain.UseCases.SignOut;

namespace TFA.Forum.Storage.Storages;

public class SignOutStorage : ISignOutStorage
{
    public Task RemoveSession(Guid sessionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
