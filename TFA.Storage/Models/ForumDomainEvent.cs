﻿namespace TFA.Forum.Storage.Models;

public enum ForumDomainEventType
{
    TopicCreated = 100,
    TopicUpdated = 101,
    TopicDeleted = 102,

    CommentCreated = 200,
    CommentUpdated = 201,
    CommentDeleted = 202,

}

public class ForumDomainEvent
{
    public ForumDomainEventType EventType { get; set; }

    public Guid TopicId { get; set; }

    public string Title { get; set; } = null!;

    public ForumComment Comment { get; set; }

    public class ForumComment
    {
        public Guid CommentId { get; set; }

        public string Text { get; set; } = null!;
    }
}


