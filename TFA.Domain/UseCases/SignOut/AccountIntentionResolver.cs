using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Authorization;

namespace TFA.Forum.Domain.UseCases.SignOut;

public class AccountIntentionResolver : IIntentionResolver<AccountIntention>
{
    public bool IsAllowed(IIdentity subject, AccountIntention intention)
    {
        return intention switch
        {
            AccountIntention.SignOut => !subject.IsAuthenticated(),
            _ => false
        };
    }
}
