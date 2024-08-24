using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Authorization;

namespace TFA.Forum.Domain.UseCases.CreateTopic;

internal class TopicIntentionalResolver : IIntentionResolver<TopicIntention>
{
    public bool IsAllowed(IIdentity subject, TopicIntention intention) => intention switch
    {
        TopicIntention.Create => subject.IsAuthenticated(),
        _ => false,
    };
}
