using Microsoft.AspNetCore.Mvc;
using TFA.API.IAuthentication;
using TFA.API.Models;
using TFA.Domain.UseCases.SignIn;
using TFA.Domain.UseCases.SignOn;

namespace TFA.API.Controllers;

[ApiController]
[Route("account")]
public class AccountController :ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SignOn(
        [FromBody] SignOn request,
        [FromServices] ISignOnUseCase signOnUseCase,
        CancellationToken cancellationToken)
    {
        var identity = await signOnUseCase.Execute(new(request.Login, request.Password), cancellationToken);
        return Ok(identity);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(
        [FromBody] SignIn request,
        [FromServices] ISignInUseCase signInUseCase,
        [FromServices] IAuthTokenStorage tokenStorage,
        CancellationToken cancellationToken)
    {
        var (identity, token)= await signInUseCase.Execute(
            new(request.Login, request.Password), cancellationToken);

        tokenStorage.Store(HttpContext, token);

        return Ok(identity);
    }
}
