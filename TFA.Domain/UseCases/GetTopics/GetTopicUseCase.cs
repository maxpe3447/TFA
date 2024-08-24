using FluentValidation;
using MediatR;
using TFA.Forum.Domain.Models;
using TFA.Forum.Domain.UseCases.GetForums;

namespace TFA.Forum.Domain.UseCases.GetTopics;

internal class GetTopicUseCase : IRequestHandler<GetTopicsQuery, (IEnumerable<Topic> resource, int totalCount)>
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
        storage = getTopicsStorage;
        this.getForumsStorage = getForumsStorage;
    }

    public async Task<(IEnumerable<Topic> resource, int totalCount)> Handle(GetTopicsQuery query, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);
        await getForumsStorage.ThrowIfForumNotFound(query.ForumId, cancellationToken);
        return await storage.GetTopics(query.ForumId, query.Skip, query.Take, cancellationToken);
    }
}
