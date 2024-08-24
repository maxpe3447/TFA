using MediatR;
using TFA.Search.Domain.Models;

namespace TFA.Search.Domain.UseCases.Search;

internal class SearchUseCase(ISearchStorage searchStorage)
    : IRequestHandler<SearchQuery, (IEnumerable<SearchResult> response, int totalCount)>
{
    public Task<(IEnumerable<SearchResult> response, int totalCount)> Handle(
        SearchQuery request, CancellationToken cancellationToken) =>
        searchStorage.Search(request.Query, cancellationToken);
}
