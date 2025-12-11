using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Devices.Application.Identities;
using Backbone.Modules.Devices.Application.Identities.Commands.DeleteIdentity;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.DeletePnsRegistrationsOfIdentity;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using MediatR;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities;

public class IdentityDeleterTests : AbstractTestsBase
{
    [Fact]
    public async Task Deleter_calls_correct_command()
    {
        var hash = Hasher.HashUtf8("did:e:nmshd-bkb.meinbildungsraum.de:dids:5812d78f3e82732df122f3");
        var base64 = Convert.ToBase64String(hash);
        
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var dummyIDeletionProcessLogger = A.Dummy<IDeletionProcessLogger>();
        var deleter = new IdentityDeleter(mockMediator, dummyIDeletionProcessLogger);
        var identityAddress = CreateRandomIdentityAddress();

        // Act
        await deleter.Delete(identityAddress);

        // Assert
        A.CallTo(() => mockMediator.Send(A<DeleteIdentityCommand>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => mockMediator.Send(A<DeletePnsRegistrationsOfIdentityCommand>.That.Matches(command => command.IdentityAddress == identityAddress), A<CancellationToken>._)).MustHaveHappened();
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
