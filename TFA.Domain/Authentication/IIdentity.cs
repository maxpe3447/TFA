namespace TFA.Domain.Authentication;

public interface IIdentity
{
    Guid UserId { get; }
}

public class User(Guid guid) : IIdentity
{
    public Guid UserId => guid;
    public static User Guest => new(Guid.Empty);
}
internal static class IdentityExtensions
{
    public static bool IsAuthenticated(this IIdentity identity) => identity.UserId != Guid.Empty;
}