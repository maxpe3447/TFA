using TFA.Domain.UseCases.SignOut;

namespace TFA.Storage.Storages;

public class SignOutStorage : ISignOutStorage
{
    public Task RemoveSession(Guid sessionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
