using DeleteDatawalletsOfIdentitySlice = Backbone.Modules.Synchronization.Module.Features.Datawallets.DeleteDatawalletsOfIdentity;
using DeleteExternalEventsOfIdentitySlice = Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteExternalEventsOfIdentity;
using DeleteSyncRunsOfIdentitySlice = Backbone.Modules.Synchronization.Module.Features.SyncRuns.DeleteSyncRunsOfIdentity;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Synchronization.Module.Features.Identities.DeleteIdentity;
using FakeItEasy;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Tests.Features.Identities.DeleteIdentity;

public class IdentityDeleterTests : AbstractTestsBase
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var dummyIDeletionProcessLogger = A.Dummy<IDeletionProcessLogger>();
        var deleter = new IdentityDeleter(mockMediator, dummyIDeletionProcessLogger);
        var identityAddress = CreateRandomIdentityAddress();

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mockMediator.Send(A<DeleteExternalEventsOfIdentitySlice.Command>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => mockMediator.Send(A<DeleteSyncRunsOfIdentitySlice.Command>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => mockMediator.Send(A<DeleteDatawalletsOfIdentitySlice.Command>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Deleter_correctly_creates_audit_log()
    {
        // Arrange
        var dummyMediator = A.Dummy<IMediator>();
        var mockIDeletionProcessLogger = A.Fake<IDeletionProcessLogger>();
        var deleter = new IdentityDeleter(dummyMediator, mockIDeletionProcessLogger);
        var identityAddress = CreateRandomIdentityAddress();

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "ExternalEvents")).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "SyncRuns")).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "Datawallets")).MustHaveHappenedOnceExactly();
    }
}
