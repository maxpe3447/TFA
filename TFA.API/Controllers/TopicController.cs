using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TFA.Forum.API.Models;
using TFA.Forum.Domain.UseCases.CreateComment;

namespace TFA.Forum.API.Controllers;

[ApiController]
[Route("topics")]
public class TopicController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost("{topicId:guid}/comments")]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(201, Type = typeof(Models.Comment))]
    public async Task<ActionResult> CreateComment(
        [FromRoute] Guid topicId,
        [FromBody] CreateForum request,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = new CreateCommentCommand(topicId, request.Title);
        var comment = await mediator.Send(command, cancellationToken);

        return Ok(mapper.Map<Models.Comment>(comment));
    }
}
