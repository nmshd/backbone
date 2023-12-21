using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsByIdentity;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Hosting;
using Xunit;
using IdentityDeletionWorker = Backbone.Modules.Devices.Jobs.IdentityDeletion.Worker;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion.Tests.Tests.Worker;
public class WorkerTests
{
    [Fact]
    public async Task Worker_Calls_Command_To_Get_Identities()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();

        RegisterCommand(mediator);

        var identityDeleters = new List<IIdentityDeleter>();
        var worker = GetWorker(mediator, identityDeleters);

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => mediator.Send(A<UpdateDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static IdentityDeletionWorker GetWorker(IMediator mediator, List<IIdentityDeleter> identityDeleters)
    {
        return new IdentityDeletionWorker(A.Dummy<IHostApplicationLifetime>(), identityDeleters, mediator, A.Dummy<IPushNotificationSender>());
    }

    [Fact]
    public async Task Worker_Calls_Deletors_For_Each_Identity()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();
        RegisterCommand(mediator, identityAddress1, identityAddress2);
        var identityDeleter = A.Dummy<IIdentityDeleter>();
        var identityDeleters = new List<IIdentityDeleter>([identityDeleter]);

        var worker = GetWorker(mediator, identityDeleters);

        A.CallTo(() => mediator.Send(A<FindRelationshipsByIdentityCommand>._, A<CancellationToken>._)).Returns(new FindRelationshipsByIdentityResponse() { Relationships = new List<Relationship>() });

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => identityDeleter.Delete(A<IdentityAddress>._)).MustHaveHappened(2, Times.Exactly);
    }

    private void RegisterCommand(IMediator mediator, params string[] identityAddresses)
    {
        var commandResponse = new UpdateDeletionProcessesResponse
        {
            IdentityAddresses = [.. identityAddresses]
        };

        A.CallTo(() => mediator.Send(A<UpdateDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);
    }
}
