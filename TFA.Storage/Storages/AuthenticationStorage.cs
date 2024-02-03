using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.Authentication;

namespace TFA.Storage.Storages;

internal class AuthenticationStorage : IAunthenticationStorage
{
    private readonly ForumDbContext dbContext;
    private readonly IMapper mapper;

    public AuthenticationStorage(
        ForumDbContext dbContext,
        IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }
    public Task<RecognisedUser?> FindUser(string login, CancellationToken cancellationToken)
    {
        return dbContext.Users
            .Where(u => u.Login.Equals(login))
            .ProjectTo<RecognisedUser>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken) ;
    }
}
