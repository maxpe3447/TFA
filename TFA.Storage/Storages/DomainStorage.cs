using AutoMapper;
using System.Diagnostics;
using System.Text.Json;
using TFA.Forum.Domain;
using TFA.Forum.Domain.UseCases;
using TFA.Forum.Storage.Entities;
using TFA.Forum.Storage.Models;

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
        var storageDomainEvent = mapper.Map<ForumDomainEvent>(domainEntity);

        await forumDbContext.DomainEvents.AddAsync(new DomainEvent
        {
            DomainEventId = guidFactory.Create(),
            EmittedAt = momentProvider.Now,
            ContentBlob = JsonSerializer.SerializeToUtf8Bytes(domainEntity),
            ActivityId = Activity.Current?.Id

        }, cancellationToken);

        await forumDbContext.SaveChangesAsync(cancellationToken);
    }
}
