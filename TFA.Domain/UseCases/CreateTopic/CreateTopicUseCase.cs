using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFA.Domain.Models;
using TFA.Storage;

namespace TFA.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly ForumDbContext forumDbContext;

    public CreateTopicUseCase(ForumDbContext forumDbContext)
    {
        this.forumDbContext = forumDbContext;
    }
    public Task<Topic> Execute(Guid forumId, string title, Guid authorId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
