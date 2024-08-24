using AutoMapper;
using TFA.Forum.Domain.Models;

namespace TFA.API.Mapping
{
    public class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<Forum, Models.Forum>();
            CreateMap<Topic, Models.Topic>();
        }
    }
}
