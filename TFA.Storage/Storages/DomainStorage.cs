using System.Text.Json;
using TFA.Domain;
using TFA.Domain.UseCases;

namespace TFA.Storage.Storages;

internal class DomainStorage(
    ForumDbContext forumDbContext,
    IGuidFactory guidFactory,
    IMomentProvider momentProvider) : IDomainEventStorage, IStorage
{
    public async Task AddEvent<TDomainEntity>(TDomainEntity domainEntity, CancellationToken cancellationToken)
    {
        await forumDbContext.DomainEvents.AddAsync(new DomainEvent
        {
            DomainEventId = guidFactory.Create(),
            EmittedAt = momentProvider.Now,
            ContentBlob = JsonSerializer.SerializeToUtf8Bytes(domainEntity),
        }, cancellationToken);

        await forumDbContext.SaveChangesAsync(cancellationToken);
    }
}
