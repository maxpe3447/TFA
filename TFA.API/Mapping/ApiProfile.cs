using AutoMapper;
using TFA.Forum.Domain.Models;

namespace TFA.Forum.API.Mapping;

public class ApiProfile : Profile
{
    public ApiProfile()
    {
        CreateMap<Domain.Models.Forum, Models.Forum>();
        CreateMap<Topic, Models.Topic>();
        CreateMap<Comment, Models.Comment>();
    }
}
