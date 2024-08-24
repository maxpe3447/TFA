using FluentAssertions;
using TFA.Forum.Domain.UseCases.GetTopics;

namespace TFA.Forum.Domain.Tests.GetTopics;

public class GetTopicQueryValidatorShould
{
    private readonly GetTopicsQueryValidator sut = new();
    [Fact]
    public void ReturnSuccess_WhenQueryIsValid()
    {
        var query = new GetTopicsQuery(Guid.Parse("1df0d982-4114-4be6-b6be-0e0d6fccf121"), 10, 5);
        sut.Validate(query).IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidQuery()
    {
        var query = new GetTopicsQuery(Guid.Parse("1df0d982-4114-4be6-b6be-0e0d6fccf121"), 10, 5);

        yield return new object[] { query with { ForumId = Guid.Empty } };
        yield return new object[] { query with { Skip = -10 } };
        yield return new object[] { query with { Take = -1 } };
    }

    [Theory]
    [MemberData(nameof(GetInvalidQuery))]
    public void ReturnFailure_WhenQueryIsInvalid(GetTopicsQuery query)
    {
        sut.Validate(query).IsValid.Should().BeFalse();
    }
}
