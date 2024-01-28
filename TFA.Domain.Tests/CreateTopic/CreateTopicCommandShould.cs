using FluentAssertions;
using TFA.Domain.UseCases.CreateTopic;
using Xunit.Sdk;

namespace TFA.Domain.Tests.CreateTopic;

public class CreateTopicCommandShould
{
    private readonly CreateTopicCommandValidator sut = new();

    [Fact]
    public void ReturnSuccess_WhenCommandIsValid()
    {
        var actual = sut.Validate(new CreateTopicCommand(Guid.Parse("3e7b334a-b880-4720-8b33-3aba68928d9a"), "Hello"));
        actual.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var validCommand = new CreateTopicCommand(Guid.Parse("a101fb92-d1be-4c2f-88dc-348905a462aa"), "Hello");

        yield return new object[] { validCommand with { ForumId = Guid.Empty }/*, nameof(CreateTopicCommand.ForumId), "Empty" */};
        yield return new object[] { validCommand with { Title = null }/*, nameof(CreateTopicCommand.Title), "Empty" */};
        yield return new object[] { validCommand with { Title = "" }/*, nameof(CreateTopicCommand.Title), "Empty"*/ };
        yield return new object[] { validCommand with { Title = "      " }/*, nameof(CreateTopicCommand.Title), "Empty"*/ };
        yield return new object[] { validCommand with { Title = new string('a', 200) }/*, nameof(CreateTopicCommand.Title), "TooLong" */};
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void ReturnFailure_WhenCommandIsValid(CreateTopicCommand command/*, string expectedInvalidPropertyName, string expectedErrorCode*/)
    {
        var actual = sut.Validate(command);
        actual.IsValid.Should().BeFalse();
        //actual.Errors.Should()
        //    .Contain(f=>f.PropertyName == expectedInvalidPropertyName && f.ErrorCode == expectedErrorCode);
    }
}
