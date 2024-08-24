using MediatR;
using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Authorization;

namespace TFA.Forum.Domain.UseCases.SignOut;


internal class SignOutUseCase : IRequestHandler<SignOutCommand>
{
    private readonly IIdentityProvider identityProvider;
    private readonly ISignOutStorage signOutStorage;
    private readonly IIntentionManager intentionManager;

    public SignOutUseCase(IIdentityProvider identityProvider,
                          ISignOutStorage signOutStorage,
                          IIntentionManager intentionManager)
    {
        this.identityProvider = identityProvider;
        this.signOutStorage = signOutStorage;
        this.intentionManager = intentionManager;
    }

    public async Task Handle(SignOutCommand command, CancellationToken cancellationToken)
    {
        intentionManager.ThrowIfForbidden(AccountIntention.SignOut);

        var sessionId = identityProvider.Current.SessionId;
        await signOutStorage.RemoveSession(sessionId, cancellationToken);
    }
}
