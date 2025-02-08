using AutoMapper;
using TFA.Forum.Domain.DomainEvents;

namespace TFA.Forum.Storage.Mapping;

internal class DomainEventProfile : Profile
{
    public DomainEventProfile()
    {
        CreateMap<ForumDomainEvent, Models.ForumDomainEvent>();
        CreateMap<ForumDomainEvent.ForumComment, Models.ForumDomainEvent.ForumComment>();
    }
}
