using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using TFA.Forum.Domain.UseCases.GetForums;

namespace TFA.Forum.Domain.Tests.GetForums;

public class GetForumsUseCaseShould
{
    private readonly Mock<IGetForumsStorage> storage;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Models.Forum>>> getForumSetup;
    private readonly GetForumsUseCase sut;

    public GetForumsUseCaseShould()
    {
        storage = new Mock<IGetForumsStorage>();
        getForumSetup = storage.Setup(x => x.GetForums(It.IsAny<CancellationToken>()));

        sut = new GetForumsUseCase(storage.Object);
    }
    [Fact]
    public async Task ReturnForums_FromStorage()
    {
        var forums = new Models.Forum[]
        {
            new(){ Id=Guid.Parse("c625ad08-e267-4105-b84e-6e00288f69ef"), Title = "Test Forum"},
            new(){ Id=Guid.Parse("c625ad08-e267-4105-b84e-6e00288f69ef"), Title = "Test Forum"},
        };

        getForumSetup.ReturnsAsync(forums);

        var actual = await sut.Handle(new GetForumQuery(),CancellationToken.None);
        actual.Should().BeSameAs(forums);

        storage.Verify(s => s.GetForums(CancellationToken.None), Times.Once);
        storage.VerifyNoOtherCalls();

    }
}
