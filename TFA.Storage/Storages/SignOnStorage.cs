using TFA.Domain.UseCases.SignOn;

namespace TFA.Storage.Storages;

internal class SignOnStorage : ISignOnStorage
{
    private readonly ForumDbContext forumDbContext;
    private readonly IGuidFactory guidFactory;

    public SignOnStorage(
        ForumDbContext forumDbContext,
        IGuidFactory guidFactory)
    {
        this.forumDbContext = forumDbContext;
        this.guidFactory = guidFactory;
    }
    public async Task<Guid> CreateUser(string login, byte[] salt, byte[] hash, CancellationToken cancellationToken)
    {
        var userId = guidFactory.Create();

        await forumDbContext.Users.AddAsync(new Entities.User
        {
            Login = login,
            Salt = salt,
            PasswordHash = hash
        }, cancellationToken);

        await forumDbContext.SaveChangesAsync();

        return userId;
    }
}
