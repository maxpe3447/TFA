using AutoMapper;

namespace TFA.Forum.Storage.Mapping;

internal class ForumProfile : Profile
{
    public ForumProfile()
    {
        CreateMap<Entities.Forum, Domain.Models.Forum>()
            .ForMember(d => d.Id, s => s.MapFrom(f => f.ForumId))
            .ForMember(d => d.Title, s => s.MapFrom(f => f.Title));
    }
}
