using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TFA.Storage;

namespace TFA.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ForumController :ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(string[]))]
        public async Task<IActionResult> GetForums([FromServices] ForumDbContext dbContext, CancellationToken cancellationToken)
        {
            return Ok(await dbContext.Forums.Select(f => f.Title).ToArrayAsync(cancellationToken));
        }
    }
}
