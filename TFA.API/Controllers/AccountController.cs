using MediatR;
using Microsoft.AspNetCore.Mvc;
using TFA.API.IAuthentication;
using TFA.Forum.API.Models;
using TFA.Forum.Domain.UseCases.SignIn;
using TFA.Forum.Domain.UseCases.SignOn;

namespace TFA.Forum.API.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly IMediator mediator;

    public AccountController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> SignOn(
        [FromBody] SignOn request,
        CancellationToken cancellationToken)
    {
        var identity = await mediator.Send(new SignOnCommand(request.Login, request.Password), cancellationToken);
        return Ok(identity);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(
        [FromBody] SignIn request,
        [FromServices] IAuthTokenStorage tokenStorage,
        CancellationToken cancellationToken)
    {
        var (identity, token) = await mediator.Send(
            new SignInCommand(request.Login, request.Password), cancellationToken);

        tokenStorage.Store(HttpContext, token);

        return Ok(identity);
    }
}
