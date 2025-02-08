namespace TFA.Forum.Domain.Models;

internal class TopicListItem
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Title { get; set; }
    public int TotalCommentCount { get; set; }
    public TopicsListLastComment? LastComment { get; set; }
}

public class TopicsListLastComment
{
    public Guid? Id { get; set; }
    public DateTimeOffset DateTimeOffset { get; set; }
}
