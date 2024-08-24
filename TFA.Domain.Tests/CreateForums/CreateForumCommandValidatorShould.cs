using FluentAssertions;
using TFA.Forum.Domain.UseCases.CreateForum;

namespace TFA.Forum.Domain.Tests.CreateForums;

public class CreateForumCommandValidatorShould
{
    private readonly CreateForumCommandValidator sut = new();

    [Fact]
    public void ReturnSuccess_WhenCommandValid()
    {
        var validCommand = new CreateForumCommand("Title");
        sut.Validate(validCommand).IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidCommand()
    {
        var validCommand = new CreateForumCommand("Title");

        yield return new object[] { validCommand with { Title = string.Empty } };
        yield return new object[] { validCommand with { Title = new string('a', 60) } };
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommand))]
    public void ReturnFailure_WhenCommandInvalid(CreateForumCommand command)
    {
        sut.Validate(command).IsValid.Should().BeFalse();
    }
}
