using AutoMapper;
using System.Text.Json;
using TFA.Forum.Domain;
using TFA.Forum.Domain.UseCases;
using TFA.Forum.Storage.Entities;

namespace TFA.Forum.Storage.Storages;

internal class DomainStorage(
    ForumDbContext forumDbContext,
    IGuidFactory guidFactory,
    IMomentProvider momentProvider,
    IMapper mapper) : IDomainEventStorage, IStorage
{
    private readonly IMapper mapper = mapper;

    public async Task AddEvent(Domain.DomainEvents.ForumDomainEvent domainEntity, CancellationToken cancellationToken)
    {
        var storageDomainEvent = mapper.Map<Entities.ForumDomainEvent>(domainEntity);

        await forumDbContext.DomainEvents.AddAsync(new DomainEvent
        {
            DomainEventId = guidFactory.Create(),
            EmittedAt = momentProvider.Now,
            ContentBlob = JsonSerializer.SerializeToUtf8Bytes(domainEntity),
        }, cancellationToken);

        await forumDbContext.SaveChangesAsync(cancellationToken);
    }
}
