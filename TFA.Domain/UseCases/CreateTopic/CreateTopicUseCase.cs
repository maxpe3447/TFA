using TFA.Domain.Exceptions;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly ICreateTopicStorage storage;

    public CreateTopicUseCase(ICreateTopicStorage storage)
    {
        this.storage = storage;
    }
    public async Task<Topic> Execute(Guid forumId, string title, Guid authorId, CancellationToken cancellationToken)
    {
        var forumExists = await storage.ForumExists(forumId, cancellationToken);
        if (!forumExists)
        {
            throw new ForumNotFoundException(forumId);
        }
        return await storage.CreateTopic(forumId, authorId, title, cancellationToken);
    }
}
