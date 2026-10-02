using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Devices.Module.Features.Identities.DeleteIdentity;
using FakeItEasy;
using MediatR;
using DeleteIdentitySlice = Backbone.Modules.Devices.Module.Features.Identities.DeleteIdentity;
using DeletePnsRegistrationsOfIdentitySlice = Backbone.Modules.Devices.Module.Features.PushNotifications.DeletePnsRegistrationsOfIdentity;

namespace Backbone.Modules.Devices.Module.Tests.Features.Identities.DeleteIdentity;

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
        A.CallTo(() => mockMediator.Send(A<DeleteIdentitySlice.Command>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => mockMediator.Send(A<DeletePnsRegistrationsOfIdentitySlice.Command>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
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
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "Identities")).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIDeletionProcessLogger.LogDeletion(identityAddress, "PnsRegistrations")).MustHaveHappenedOnceExactly();
    }
}
