using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Synchronization.Application.Datawallets.Commands.DeleteDatawalletsOfIdentity;
using Backbone.Modules.Synchronization.Application.Identities;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteExternalEventsOfIdentity;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.DeleteSyncRunsOfIdentity;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using MediatR;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.Identities;
public class IdentityDeleterTests : AbstractTestsBase
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var deleter = new IdentityDeleter(mockMediator);
        var identityAddress = CreateRandomIdentityAddress();
        var dummyIDeletionProcessLogger = A.Fake<IDeletionProcessLogger>();

        // Act
        await deleter.Delete(identityAddress, dummyIDeletionProcessLogger);

        // Assert
        A.CallTo(() => mockMediator.Send(A<DeleteExternalEventsOfIdentityCommand>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => mockMediator.Send(A<DeleteSyncRunsOfIdentityCommand>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => mockMediator.Send(A<DeleteDatawalletsOfIdentityCommand>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Deleter_correctly_creates_audit_log()
    {
        // Arrange
        var dummyMediator = A.Fake<IMediator>();
        var deleter = new IdentityDeleter(dummyMediator);
        var identityAddress = CreateRandomIdentityAddress();
        var mockIDeletionProcessLogger = A.Fake<IDeletionProcessLogger>();

        // Act
        await deleter.Delete(identityAddress, mockIDeletionProcessLogger);

        // Assert
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, AggregateType.ExternalEvents)).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, AggregateType.SyncRuns)).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, AggregateType.Datawallets)).MustHaveHappenedOnceExactly();
    }
}
