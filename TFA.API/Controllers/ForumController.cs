using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TFA.Forum.API.Models;
using TFA.Forum.Domain.UseCases.CreateForum;
using TFA.Forum.Domain.UseCases.CreateTopic;
using TFA.Forum.Domain.UseCases.GetForums;
using TFA.Forum.Domain.UseCases.GetTopics;
using TFA.Search.API.Grpc;

namespace TFA.Forum.API.Controllers;

[ApiController]
[Route("forums")]
public class ForumController(IMediator mediator) : ControllerBase
{
    private readonly IMediator mediator = mediator;

    [HttpPost]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(201, Type = typeof(Models.Forum))]
    public async Task<ActionResult> CreateForum(
        [FromBody] CreateForum request,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = new CreateForumCommand(request.Title);
        var forum = await mediator.Send(command, cancellationToken);

        return CreatedAtRoute(nameof(GetForums), mapper.Map<Models.Forum>(forum));
    }

    [HttpGet(Name = nameof(GetForums))]
    [ProducesResponseType(200, Type = typeof(
        Models.Forum[]))]
    public async Task<IActionResult> GetForums(
        [FromServices] IMapper mapper,
        [FromServices] SearchEngine.SearchEngineClient client,
        CancellationToken cancellationToken)
    {
        await client.SearchAsync(new SearchRequest
        {
            Query = "hello"
        }, cancellationToken: cancellationToken);

        var forums = await mediator.Send(new GetForumQuery(), cancellationToken);
        return Ok(forums.Select(mapper.Map<Models.Forum>));
    }
    [HttpPost("{forumId}/topics")]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(410)]
    [ProducesResponseType(201, Type = typeof(Topic))]
    public async Task<IActionResult> CreateTopic(
        Guid forumId,
        [FromBody] CreateTopic request,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = new CreateTopicCommand(forumId, request.Title);
        var topic = await mediator.Send(command, cancellationToken);

        return CreatedAtRoute(nameof(GetForums), mapper.Map<Topic>(topic));
    }

    [HttpGet("{forumId:guid}/topics")]
    public async Task<IActionResult> GetTopics(
        [FromRoute] Guid forumId,
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var (resources, totalCount) = await mediator.Send(new GetTopicsQuery(forumId, skip, take), cancellationToken);

        return Ok(new
        {
            resuorces = resources.Select(mapper.Map<Topic>),
            totalCount
        });
    }
}
