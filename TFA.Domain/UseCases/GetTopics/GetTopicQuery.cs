namespace TFA.Domain.UseCases.GetTopics;

public record GetTopicQuery(Guid ForumId, int Skip, int Take);
