using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFA.API.Models;
using TFA.Storage;

namespace TFA.API.Controllers
{
    [ApiController]
    [Route("forums")]
    public class ForumController :ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Forum[]))]
        public async Task<IActionResult> GetForums([FromServices] ForumDbContext dbContext, CancellationToken cancellationToken)
        {
            return Ok(await dbContext.Forums
                .Select(f => new Forum
                {
                    Id = f.ForumId,
                    Title = f.Title
                })
                .ToArrayAsync(cancellationToken));
        }
    }
}
