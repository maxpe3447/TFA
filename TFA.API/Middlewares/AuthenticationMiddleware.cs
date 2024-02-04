using TFA.API.IAuthentication;
using TFA.Domain.Authentication;

namespace TFA.API.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate next;
    public AuthenticationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuthTokenStorage authTokenStorage,
        IAuthenticationService authenticationService,
        IIdentitySetter identitySetter,
        CancellationToken cancellationToken)
    {
        var identity = authTokenStorage.TryExtract(context, out var authToken)
            ? await authenticationService.Authenticate(authToken, cancellationToken)
            : User.Guest;

        identitySetter.Current = identity;

        await next(context);
    }
}
