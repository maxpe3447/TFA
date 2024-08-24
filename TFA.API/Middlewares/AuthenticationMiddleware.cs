using TFA.API.IAuthentication;
using TFA.Domain.Authentication;
using TFA.Forum.Domain.Authentication;

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
        IIdentityProvider identityProvider)
    {
        var identity = authTokenStorage.TryExtract(context, out var authToken)
            ? await authenticationService.Authenticate(authToken, CancellationToken.None)
            : User.Guest;

        identityProvider.Current = identity;

        Console.WriteLine(identity);

        await next(context);
    }
}
