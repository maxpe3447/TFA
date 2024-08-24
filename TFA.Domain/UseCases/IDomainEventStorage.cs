using TFA.Forum.Domain.DomainEvents;

namespace TFA.Forum.Domain.UseCases;

public interface IDomainEventStorage : IStorage
{
    Task AddEvent(ForumDomainEvent domainEntity, CancellationToken cancellationToken);
}
