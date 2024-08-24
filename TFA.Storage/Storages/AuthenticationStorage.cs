using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Forum.Domain.Authentication;

namespace TFA.Forum.Storage.Storages;

public class AuthenticationStorage : IAuthenticationStorage
{
    private readonly ForumDbContext forumDbContext;
    private readonly IMapper mapper;

    public AuthenticationStorage(
        ForumDbContext forumDbContext,
        IMapper mapper)
    {
        this.forumDbContext = forumDbContext;
        this.mapper = mapper;
    }

    public Task<Session?> FindSessionId(Guid sessionId, CancellationToken cancellationToken)
    {
        return forumDbContext.Sessions
            .Where(s => s.SessionId == sessionId)
            .ProjectTo<Session>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
