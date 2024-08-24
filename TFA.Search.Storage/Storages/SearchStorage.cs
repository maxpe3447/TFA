using OpenSearch.Client;
using TFA.Search.Domain.Models;
using TFA.Search.Domain.UseCases.Search;

namespace TFA.Search.Storage.Storages;

internal class SearchStorage(IOpenSearchClient client) : ISearchStorage
{
    public async Task<(IEnumerable<SearchResult> response, int totalCount)> Search(
        string query, CancellationToken cancellationToken)
    {
        try
        {

            var searchResponse = await client.SearchAsync<Entities.SearchEntity>(description => description
            .Query(q => q
                .Bool(b => b
                    .Should(
                        s => s.Match(m => m.Field(se => se.Title).Query(query)),
                        s => s.Match(m => m.Field(se => se.Text).Query(query).Fuzziness(Fuzziness.EditDistance(1)))
                    )
                )
            ).Highlight(h => h
                .Fields(
                    f => f.Field(se => se.Title),
                    f => f.Field(se => se.Text).PreTags("<mark>").PostTags("</mark>")
                )), cancellationToken);

            return (searchResponse.Hits.Select(hit => new SearchResult
            {
                EntityId = hit.Source.EntityId,
                EntityType = (SearchEntityType)hit.Source.EntityType,
                Highlights = hit.Highlight.Values.SelectMany(v => v).ToArray(),
            }).ToArray(), (int)searchResponse.Total);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
