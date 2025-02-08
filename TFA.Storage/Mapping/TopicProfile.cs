using AutoMapper;
using TFA.Forum.Storage.Models;

namespace TFA.Forum.Storage.Mapping;

public class TopicProfile : Profile
{
    public TopicProfile()
    {
        CreateMap<Entities.Topic, Domain.Models.Topic>()
            .ForMember(d => d.Id, s => s.MapFrom(t => t.TopicId));

        CreateMap<TopicListItemReadModel, Domain.Models.Topic>()
            .ForMember(d => d.Id, s => s.MapFrom(t => t.TopicId));
    }
}
