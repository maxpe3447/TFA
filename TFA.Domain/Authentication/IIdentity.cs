namespace TFA.Domain.Authentication;

public interface IIdentity
{
    Guid UserId { get; }
}

public class User : IIdentity
{
    private readonly Guid guid;

    public User(Guid guid)
    {
        this.guid = guid;
    }
    public User()
    {
        
    }
    public Guid UserId => guid;
    public static User Guest => new(Guid.Empty);
}
internal static class IdentityExtensions
{
    public static bool IsAuthenticated(this IIdentity identity) => identity.UserId != Guid.Empty;
}