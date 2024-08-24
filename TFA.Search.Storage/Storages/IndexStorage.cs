using OpenSearch.Client;
using TFA.Search.Domain.Models;
using TFA.Search.Domain.UseCases.Index;

namespace TFA.Search.Storage.Storages;

internal class IndexStorage(IOpenSearchClient client) : IIndexStorage
{
    public async Task Index(Guid entityId, SearchEntityType entityType, string? title, string? text, CancellationToken cancellationToken)
    {
        await client.IndexAsync(new Entities.SearchEntity
        {
            EntityId = entityId,
            EntityType = (int)entityType,
            Title = title,
            Text = text,

        },
       descriptor => descriptor.Index("tfa-search-v1"), cancellationToken);
    }
}
