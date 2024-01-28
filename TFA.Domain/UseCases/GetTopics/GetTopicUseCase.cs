using FluentValidation;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetTopics;

internal class GetTopicUseCase : IGetTopicsUseCase
{
    private readonly IValidator<GetTopicsQuery> validator;
    private readonly IGetTopicsStorage storage;

    public GetTopicUseCase(
        IValidator<GetTopicsQuery> validator,
        IGetTopicsStorage getTopicsStorage)
    {
        this.validator = validator;
        this.storage = getTopicsStorage;
    }
    public async Task<(IEnumerable<Topic> resource, int totalCount)> Execute(GetTopicsQuery query, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);

        return await storage.GetTopics(query.ForumId, query.Skip, query.Take, cancellationToken);
    }
}
