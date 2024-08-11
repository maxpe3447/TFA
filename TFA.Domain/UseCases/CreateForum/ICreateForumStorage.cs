using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateForum;

public interface ICreateForumStorage : IStorage
{
    public Task<Forum> Create(string title, CancellationToken cancellationToken);
}
