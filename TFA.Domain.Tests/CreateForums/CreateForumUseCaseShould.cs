using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using TFA.Forum.Domain.Authorization;
using TFA.Forum.Domain.UseCases.CreateForum;

namespace TFA.Forum.Domain.Tests.CreateForums;

public class CreateForumUseCaseShould
{
    private readonly Mock<ICreateForumStorage> storage;
    private readonly ISetup<ICreateForumStorage, Task<Models.Forum>> createForumSetup;
    private readonly CreateForumUseCase sut;

    public CreateForumUseCaseShould()
    {
        var validator = new Mock<IValidator<CreateForumCommand>>();
        validator
            .Setup(x => x.ValidateAsync(It.IsAny<CreateForumCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var intentionManager = new Mock<IIntentionManager>();
        intentionManager
            .Setup(x => x.IsAllowed(It.IsAny<ForumIntention>()))
            .Returns(true);

        storage = new Mock<ICreateForumStorage>();
        createForumSetup = storage.Setup(s => s.Create(It.IsAny<string>(), It.IsAny<CancellationToken>()));

        sut = new CreateForumUseCase(
            validator.Object, intentionManager.Object, storage.Object);
    }


    [Fact]
    public async Task ReturnCreatedForum()
    {
        var forum = new Models.Forum
        {
            Id = Guid.Parse("ec5e548a-04ab-4b97-b223-325be04abf9d"),
            Title = "Hello forum"
        };

        createForumSetup.ReturnsAsync(forum);

        var actual = await sut.Handle(new CreateForumCommand("Hello"), CancellationToken.None);
        actual.Should().Be(forum);

        storage.Verify(s => s.Create("Hello", It.IsAny<CancellationToken>()), Times.Once);
        storage.VerifyNoOtherCalls();


    }
}
