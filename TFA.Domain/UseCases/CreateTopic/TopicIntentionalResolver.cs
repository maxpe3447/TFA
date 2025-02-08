using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Authorization;
using TFA.Forum.Domain.Models;

namespace TFA.Forum.Domain.UseCases.CreateTopic;

internal class TopicIntentionalResolver :
    IIntentionResolver<TopicIntention>,
    IIntentionResolver<TopicIntention, Topic>
{
    public bool IsAllowed(IIdentity subject, TopicIntention intention) => intention switch
    {
        TopicIntention.Create => subject.IsAuthenticated(),
        _ => false,
    };

    public bool IsAllowed(IIdentity subject, TopicIntention intention, Topic target) => intention switch
    {
        TopicIntention.CreateComment => subject.IsAuthenticated(),
        _ => false
    };
}