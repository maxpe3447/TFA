namespace TFA.Domain.Authentication;

public interface IIdentity
{
    Guid UserId { get; }
}

internal class User(Guid guid) : IIdentity
{
    public Guid UserId => guid;
}
internal static class IdentityExtensions
{
    public static bool IsAuthenticated(this IIdentity identity) => identity.UserId != Guid.Empty;
}