using FluentValidation;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Domain.UseCases.GetTopics;

internal class GetTopicUseCase : IGetTopicsUseCase
{
    private readonly IValidator<GetTopicsQuery> validator;
    private readonly IGetTopicsStorage storage;
    private readonly IGetForumsStorage getForumsStorage;

    public GetTopicUseCase(
        IValidator<GetTopicsQuery> validator,
        IGetTopicsStorage getTopicsStorage,
        IGetForumsStorage getForumsStorage)
    {
        this.validator = validator;
        this.storage = getTopicsStorage;
        this.getForumsStorage = getForumsStorage;
    }
    public async Task<(IEnumerable<Topic> resource, int totalCount)> Execute(GetTopicsQuery query, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);
        await getForumsStorage.ThrowIfForumNotFound(query.ForumId, cancellationToken);
        return await storage.GetTopics(query.ForumId, query.Skip, query.Take, cancellationToken);
    }
}
