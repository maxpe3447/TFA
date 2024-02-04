using TFA.Domain.UseCases.SignIn;

namespace TFA.Domain.Authentication;

public interface IAunthenticationStorage
{
    Task<RecognisedUser?> FindUser(string login, CancellationToken cancellationToken);
}
