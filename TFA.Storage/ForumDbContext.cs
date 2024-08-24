using Microsoft.EntityFrameworkCore;
using TFA.Forum.Storage.Entities;

namespace TFA.Forum.Storage;

public class ForumDbContext : DbContext
{
    public ForumDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

    }
    public DbSet<User> Users { get; set; }
    public DbSet<Entities.Forum> Forums { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Session> Sessions { get; set; }

    public DbSet<DomainEvent> DomainEvents { get; set; }
}
