using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using TFA.API.Models;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;
using TFA.Domain.Models;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;

namespace TFA.API.Controllers
{
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
        [ProducesResponseType(403)]
        [ProducesResponseType(410)]
        [ProducesResponseType(201, Type = typeof(Models.Topic))]
        public async Task<IActionResult> CreateTopic(
            Guid forumId,
            [FromBody] CreateTopic request,
            ICreateTopicUseCase useCase,
            CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                return ex switch
                {
                    IntentionManagerException => Forbid(),
                    ForumNotFoundException => StatusCode(StatusCodes.Status410Gone),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                }; ;
            }
        }
    }
}
