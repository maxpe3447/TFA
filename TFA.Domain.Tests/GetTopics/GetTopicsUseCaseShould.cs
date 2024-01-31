using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using System.Globalization;
using TFA.Domain.Exceptions;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain.UseCases.GetTopics;
using Xunit.Sdk;

namespace TFA.Domain.Tests.GetTopics;

public class GetTopicsUseCaseShould
{
    private readonly Mock<IGetTopicsStorage> storage;
    private readonly ISetup<IGetTopicsStorage, Task<(IEnumerable<Topic> resources, int totalCount)>> getTopicsSetUp;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> getForumsSetUp;
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

        var getForumsStorage = new Mock<IGetForumsStorage>();
        getForumsSetUp = getForumsStorage.Setup(s => s.GetForums(It.IsAny<CancellationToken>()));


        sut = new GetTopicUseCase(validator.Object, storage.Object, getForumsStorage.Object);
    }
    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoForum()
    {
        var forumId = Guid.Parse("ce9b42e4-41d0-4431-9d06-3e954e7ffc17");

        getForumsSetUp.ReturnsAsync(new Forum[] { new() { Id = Guid.Parse("ee838e3f-a40b-4a90-97e5-e0af1b13a970")} });

        var query = new GetTopicsQuery(forumId, 0, 1);
        await sut.Invoking(s=>s.Execute(query, CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
    }

    [Fact]
    public async Task ReturnTopics_ExtractedFromStorage_WhenForumExists()
    {
        var forumId = Guid.Parse("9fa702cf-014a-4a36-966a-a02f7b9af3e8");
        getForumsSetUp.ReturnsAsync(new Forum[] { new() { Id = Guid.Parse("9fa702cf-014a-4a36-966a-a02f7b9af3e8") } });

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
