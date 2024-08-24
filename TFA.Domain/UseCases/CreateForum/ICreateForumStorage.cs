namespace TFA.Forum.Domain.UseCases.CreateForum;

public interface ICreateForumStorage : IStorage
{
    public Task<Models.Forum> Create(string title, CancellationToken cancellationToken);
}
