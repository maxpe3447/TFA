using MediatR;
using Microsoft.AspNetCore.Mvc;
using TFA.Search.Domain.Models;
using TFA.Search.Domain.UseCases.Index;
using TFA.Search.Domain.UseCases.Search;

namespace TFA.Search.API.Controllers;

[ApiController]
public class SearchController(IMediator mediator) :ControllerBase
{
    [HttpPost("index")]
    public async Task<IActionResult> Index(
        [FromBody] SearchEntity searchEntity,
        CancellationToken cancellationToken)
    {
       await mediator.Send(new IndexCommand(searchEntity.EntityId, searchEntity.EntityType, searchEntity.Title, searchEntity.Text), cancellationToken);

        return Ok();
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        string query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SearchQuery(query));
        return Ok(result);
    }

}
