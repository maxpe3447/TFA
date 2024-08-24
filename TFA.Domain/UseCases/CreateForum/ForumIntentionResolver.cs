using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Authorization;

namespace TFA.Forum.Domain.UseCases.CreateForum;

internal class ForumIntentionResolver : IIntentionResolver<ForumIntention>
{
    public bool IsAllowed(IIdentity subject, ForumIntention intention) => intention switch
    {
        ForumIntention.Create => subject.IsAuthenticated(),
        _ => false
    };
}
