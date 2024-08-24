using AutoMapper;
using TFA.Forum.Domain.DomainEvents;

namespace TFA.Forum.Storage.Mapping;

internal class DomainEventProfile : Profile
{
    public DomainEventProfile()
    {
        CreateMap<ForumDomainEvent, Entities.ForumDomainEvent>();
        CreateMap<ForumDomainEvent.ForumComment, Entities.ForumDomainEvent.ForumComment>();
    }
}
