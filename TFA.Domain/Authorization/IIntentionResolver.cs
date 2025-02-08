using TFA.Forum.Domain.Authentication;

namespace TFA.Forum.Domain.Authorization;

public interface IIntentionResolver { }

public interface IIntentionResolver<in TIntention> : IIntentionResolver
{
    bool IsAllowed(IIdentity subject, TIntention intention);
}

public interface IIntentionResolver<in TIntention, in TObject> : IIntentionResolver
{
    bool IsAllowed(IIdentity subject, TIntention intention, TObject target);
}