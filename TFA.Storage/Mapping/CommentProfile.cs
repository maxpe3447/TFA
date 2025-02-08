using AutoMapper;
using TFA.Forum.Storage.Entities;

namespace TFA.Forum.Storage.Mapping;

internal class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, Domain.Models.Comment>()
            .ForMember(c => c.Id, s => s.MapFrom(c => c.CommentId));
    }
}
