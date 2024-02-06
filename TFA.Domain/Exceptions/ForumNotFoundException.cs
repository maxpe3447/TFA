namespace TFA.Domain.Exceptions;

public class ForumNotFoundException : DomainException
{
    public ForumNotFoundException(Guid forumId) : base(DomainErrorCode.Gone, $"Forum with id {forumId} was not found")
    {

    }
}

public abstract class DomainException : Exception
{
    public DomainErrorCode DomainErrorCode { get; set; }
    public DomainException(DomainErrorCode errorCode, string message) : base(message)
    {
        DomainErrorCode = errorCode;
    }
}