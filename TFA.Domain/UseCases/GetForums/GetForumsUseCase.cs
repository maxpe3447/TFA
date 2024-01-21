using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFA.Domain.Models;
using TFA.Storage;

namespace TFA.Domain.UseCases.GetForums
{
    public class GetForumsUseCase : IGetForumsUseCase
    {
        private readonly ForumDbContext forumDbContext;

        public GetForumsUseCase(
            ForumDbContext forumDbContext)
        {
            this.forumDbContext = forumDbContext;
        }
        public async Task<IEnumerable<Forum>> Execute(CancellationToken cancellationToken)
        {
            return await forumDbContext.Forums
                 .Select(f => new Forum
                 {
                     Id = f.ForumId,
                     Title = f.Title
                 }).ToArrayAsync(cancellationToken);
        }
    }
}
