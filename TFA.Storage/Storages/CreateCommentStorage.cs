using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Forum.Domain.Models;
using TFA.Forum.Domain.UseCases.CreateComment;

namespace TFA.Forum.Storage.Storages;

internal class CreateCommentStorage(
    ForumDbContext dbContext,
    IMapper mapper,
    IGuidFactory guidFactory,
    IMomentProvider momentProvider) : ICreateCommentStorage
{
    public async Task<Comment> Create(Guid topicId, Guid userId, string text, CancellationToken cancellationToken)
    {
        Entities.Comment comment = new Entities.Comment
        {
            CommentId = guidFactory.Create(),
            TopicId = topicId,
            UserId = userId,
            Text = text,
            CreatedAt = momentProvider.Now,
        };
        await dbContext.Comments.AddAsync(comment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.Comments
            .AsNoTracking()
            .Where(c => c.CommentId == comment.CommentId)
            .ProjectTo<Comment>(mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
    }

    public Task<Topic?> FindTopic(Guid topicId, CancellationToken cancellationToken) => dbContext.Topics
            .AsNoTracking()
            .Where(t => t.TopicId == topicId)
            .ProjectTo<Topic?>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
}
