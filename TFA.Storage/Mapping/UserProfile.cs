using AutoMapper;
using TFA.Forum.Domain.UseCases.SignIn;
using TFA.Forum.Storage.Entities;

namespace TFA.Forum.Storage.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, RecognizedUser>();
        CreateMap<Session, Domain.Authentication.Session>();
    }
}
