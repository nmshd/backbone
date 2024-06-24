using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Files.Application.Identities;
using Backbone.Modules.Files.Application.Identities.Commands.DeleteFilesOfIdentity;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using MediatR;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Files.Application.Tests.Tests.Identities;
public class IdentityDeleterTests : AbstractTestsBase
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var dummyIDeletionProcessLogger = A.Fake<IDeletionProcessLogger>();
        var identityAddress = CreateRandomIdentityAddress();
        var deleter = new IdentityDeleter(mockMediator);

        // Act
        await deleter.Delete(identityAddress, dummyIDeletionProcessLogger);

        // Assert
        A.CallTo(() => mockMediator.Send(A<DeleteFilesOfIdentityCommand>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Deleter_correctly_creates_audit_log()
    {
        // Arrange
        var dummyMediator = A.Fake<IMediator>();
        var mockIDeletionProcessLogger = A.Fake<IDeletionProcessLogger>();
        var identityAddress = CreateRandomIdentityAddress();
        var deleter = new IdentityDeleter(dummyMediator);

        // Act
        await deleter.Delete(identityAddress, mockIDeletionProcessLogger);

        // Assert
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, AggregateType.Files)).MustHaveHappenedOnceExactly();
    }
}
