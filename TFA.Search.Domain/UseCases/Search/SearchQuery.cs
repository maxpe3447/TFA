using MediatR;
using TFA.Search.Domain.Models;

namespace TFA.Search.Domain.UseCases.Search;

public record class SearchQuery(string Query) : IRequest<(IEnumerable<SearchResult> response, int totalCount)>;
