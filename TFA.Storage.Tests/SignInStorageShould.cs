using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TFA.Forum.Storage.Entities;
using TFA.Forum.Storage.Storages;

namespace TFA.Forum.Storage.Tests;

public class SignInStorageFixture : StorageTestFixture
{
    public override async Task InitializeAsync()
    {

        await base.InitializeAsync();

        await using var dbContext = GetDbContext();
        await dbContext.Users.AddRangeAsync(
            new User
            {
                UserId = Guid.Parse("2414eeda-50b6-4c3b-bbd2-c665a1fa31a3"),
                Login = "testuser",
                Salt = [1],
                PasswordHash = [1]
            }, new Entities.User
            {
                UserId = Guid.Parse("ebc9e930-5b8b-4d68-8801-cd269280256d"),
                Login = "another user",
                Salt = [1],
                PasswordHash = [1]
            });

        await dbContext.SaveChangesAsync();
    }
}

public class SignInStorageShould(SignInStorageFixture fixture)
    : IClassFixture<SignInStorageFixture>
{
    private readonly SignInStorage sut = new SignInStorage(
            fixture.GetDbContext(),
            fixture.GetMapper(),
            new GuidFactory());

    [Fact]
    public async Task FindUserByLogin()
    {
        var userId = Guid.Parse("2414eeda-50b6-4c3b-bbd2-c665a1fa31a3");


        var actual = await sut.FindUser("testuser", CancellationToken.None);
        actual.Should().NotBeNull();
        actual.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task ReturnNewlyCreatedSessionId()
    {
        var sessionId = await sut.CreateSession(
            Guid.Parse("2414eeda-50b6-4c3b-bbd2-c665a1fa31a3"),
            new DateTimeOffset(2024, 07, 29, 1, 2, 3, TimeSpan.Zero),
            CancellationToken.None);

        await using var dbContext = fixture.GetDbContext();
        (await dbContext.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SessionId == sessionId)).Should().NotBeNull();
    }
}
