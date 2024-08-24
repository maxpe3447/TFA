using TFA.Search.Domain.Models;

namespace TFA.Search.Domain.UseCases.Search;

public interface ISearchStorage
{
    Task<(IEnumerable<SearchResult> response, int totalCount)> Search(
        string query, 
        CancellationToken cancellationToken);
}
