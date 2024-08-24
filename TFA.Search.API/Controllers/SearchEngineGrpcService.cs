using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using TFA.Search.API.Grpc;
using TFA.Search.Domain.UseCases.Index;
using TFA.Search.Domain.UseCases.Search;

namespace TFA.Search.API.Controllers;

internal class SearchEngineGrpcService(IMediator mediator) : SearchEngine.SearchEngineBase
{
    public override async Task<Empty> Index(IndexRequest request, ServerCallContext context)
    {
        var command = new IndexCommand(
            Guid.Parse(request.Id),
            request.Type switch
            {
                SearchEntityType.ForumTopic => Domain.Models.SearchEntityType.ForumTopic,
                SearchEntityType.ForumComment => Domain.Models.SearchEntityType.ForumComment,
                SearchEntityType.Unknown =>   throw new ArgumentException(),
            },
            request.Title,
            request.Text);

        await mediator.Send(command, context.CancellationToken);
        return new Empty();
    }

    public override async Task<IndexResponse> Search(SearchRequest request, ServerCallContext context)
    {
        var query = new SearchQuery(request.Query);
        var (resources, total) = await mediator.Send(query, context.CancellationToken);

        return new IndexResponse
        {
            Total = total,
            Entities = {resources.Select(r=> new IndexResponse.Types.SearchResultEntity
            {
                Id = r.EntityId.ToString(),
                Type = r.EntityType switch
                { 
                    Domain.Models.SearchEntityType.ForumTopic => SearchEntityType.ForumTopic,
                    Domain.Models.SearchEntityType.ForumComment => SearchEntityType.ForumComment, 
                    _ => SearchEntityType.Unknown
                },
                Heighkights = {r.Highlights}
            })}
        };
    }
}
