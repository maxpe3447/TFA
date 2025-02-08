namespace TFA.Forum.Domain.Exceptions;

internal class TopicNotFoundException : DomainException
{
    public TopicNotFoundException(Guid topicId)
        : base(DomainErrorCode.Gone, $"Topic with id {topicId} was not found")
    {

    }
}
