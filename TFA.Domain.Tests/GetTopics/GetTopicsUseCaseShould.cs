using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetTopics;
using Xunit.Sdk;

namespace TFA.Domain.Tests.GetTopics;

public class GetTopicsUseCaseShould
{
    private readonly Mock<IGetTopicsStorage> storage;
    private readonly ISetup<IGetTopicsStorage, Task<(IEnumerable<Topic> resources, int totalCount)>> getTopicsSetUp;
    private readonly GetTopicUseCase sut;

    public GetTopicsUseCaseShould()
    {
        var validator = new Mock<IValidator<GetTopicsQuery>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<GetTopicsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        storage = new Mock<IGetTopicsStorage>();
        getTopicsSetUp = storage
            .Setup(s => s.GetTopics(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()));

        sut = new GetTopicUseCase(validator.Object, storage.Object);
    }
    [Fact]
    public async Task ReturnTopics_ExtractedFromStorage()
    {
        var forumId = Guid.Parse("9fa702cf-014a-4a36-966a-a02f7b9af3e8");

        var expectedResources = new Topic[] { new() };
        var expectedTotalCount = 6;
        getTopicsSetUp.ReturnsAsync((expectedResources, expectedTotalCount));

        var (actualResources, actualTotalCount) = await sut.Execute(
            new GetTopicsQuery(forumId, 5, 10), CancellationToken.None);

        actualResources.Should().BeEquivalentTo(expectedResources);
        actualTotalCount.Should().Be(expectedTotalCount);

        storage.Verify(s => s.GetTopics(forumId, 5, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}
