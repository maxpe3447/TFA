using TFA.Forum.Domain.Authentication;

namespace TFA.Domain.Authentication;

public class User : IIdentity
{
    private readonly Guid userId;
    private readonly Guid sessionId;

    public User(Guid userId, Guid sessionId)
    {
        this.userId = userId;
        this.sessionId = sessionId;
    }
    public User()
    {
        
    }
    public Guid UserId => userId;
    public static User Guest => new(Guid.Empty, Guid.Empty);

    public Guid SessionId => sessionId;
}
