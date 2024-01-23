namespace TFA.Domain.Authentication;

public interface IIdentity
{
    Guid UserId { get; }
}
