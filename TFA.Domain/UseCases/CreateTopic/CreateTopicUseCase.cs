using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    public Task<Topic> Execute(Guid topicId, string title, Guid authorId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
