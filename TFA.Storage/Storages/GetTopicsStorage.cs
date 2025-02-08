using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Forum.Domain.Models;
using TFA.Forum.Domain.UseCases.GetTopics;
using TFA.Forum.Storage.Models;

namespace TFA.Forum.Storage.Storages;

internal class GetTopicsStorage(
    ForumDbContext forumDbContext,
    IMapper mapper) : IGetTopicsStorage
{
    private readonly ForumDbContext forumDbContext = forumDbContext;
    private readonly IMapper mapper = mapper;

    public async Task<(IEnumerable<Topic> resources, int totalCount)> GetTopics(Guid forumId, int skip, int take, CancellationToken cancellationToken)
    {
        var query = forumDbContext.Topics.Where(t => t.ForumId == forumId);
        var totalCount = await query.CountAsync(cancellationToken);

        var resources = await forumDbContext.Database.SqlQuery<TopicListItemReadModel>($@"
            SELECT
                t.""TopicId"" as ""TopicId"",
                t.""ForumId"" as ""ForumId"",
                t.""UserId"" as ""UserId"",
                t.""Title"" as ""Title"",
                t.""CreatedAt"" as ""CreatedAt"",
                COALESCE(c.TotalCommentsCount, 0) as ""TotalCommentsCount"",
                c.""CreatedAt"" as ""LastCommentCreatedAt"",
                c.""CommentId"" as ""LastCommentId""
            FROM ""Topics"" as t
            LEFT JOIN (
                SELECT 
                    ""TopicId"",
                    ""CommentId"",
                    ""CreatedAt"",
                    COUNT(*) OVER (PARTITION BY ""TopicId"") as TotalCommentsCount,
                    row_number() OVER (PARTITION BY ""TopicId"" ORDER BY ""CreatedAt"" DESC) as rn
                FROM ""Comments""
            ) as c ON t.""TopicId"" = c.""TopicId"" AND c.rn = 1
            WHERE t.""ForumId"" = {forumId}
            ORDER BY 
                COALESCE(c.""CreatedAt"", t.""CreatedAt"") DESC
            LIMIT {take} OFFSET {skip}")
            .ProjectTo<Topic>(mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);

        return (resources, totalCount);
    }
}
