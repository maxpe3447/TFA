﻿using Microsoft.AspNetCore.Mvc;
using TFA.API.Models;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain.UseCases.GetTopics;

namespace TFA.API.Controllers;

[ApiController]
[Route("forums")]
public class ForumController : ControllerBase
{
    [HttpGet(Name = nameof(GetForums))]
    [ProducesResponseType(200, Type = typeof(Models.Forum[]))]
    public async Task<IActionResult> GetForums([FromServices] IGetForumsUseCase useCase, CancellationToken cancellationToken)
    {
        var forums = await useCase.Execute(cancellationToken);
        return Ok(forums.Select(f => new Models.Forum
        {
            Id = f.Id,
            Title = f.Title
        }));
    }
    [HttpPost("{forumId}/topics")]
    [ProducesResponseType(400)]
    [ProducesResponseType(410)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> CreateTopic(
        Guid forumId,
        [FromBody] CreateTopic request,
        ICreateTopicUseCase useCase,
        CancellationToken cancellationToken)
    {
        var command = new CreateTopicCommand(forumId, request.Title);
        var topic = await useCase.Execute(command, cancellationToken);

        return CreatedAtRoute(nameof(GetForums), new Models.Topic
        {
            Id = topic.Id,
            Title = topic.Title,
            CreatedAt = topic.CreatedAt,
        });
    }

    [HttpGet("{forumId:guid}/topics")]
    public async Task<IActionResult> GetTopics(
        [FromRoute]Guid forumId, 
        [FromQuery] int skip, 
        [FromQuery] int take,
        [FromServices] IGetTopicsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var (resources, totalCount) = await useCase.Execute(new GetTopicsQuery(forumId, skip, take), cancellationToken);

        return Ok(new { resuorces = resources.Select(r => new Topic
        {
            Id = r.Id,
            Title = r.Title,
            CreatedAt = r.CreatedAt,
        }), totalCount});
    }
}
