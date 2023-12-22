using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsByIdentity;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Tooling;
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

        RegisterFindRipeDeletionProcessesCommand(mediator);

        var identityDeleters = new List<IIdentityDeleter>();
        var worker = GetWorker(mediator, identityDeleters, null);

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => mediator.Send(A<FindRipeDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Worker_Calls_Deletors_For_Each_Identity()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();
        RegisterFindRipeDeletionProcessesCommand(mediator, identityAddress1, identityAddress2);
        var identityDeleter = A.Dummy<IIdentityDeleter>();
        var identityDeleters = new List<IIdentityDeleter>([identityDeleter]);

        var worker = GetWorker(mediator, identityDeleters, null);

        A.CallTo(() => mediator.Send(A<FindRelationshipsByIdentityCommand>._, A<CancellationToken>._)).Returns(new FindRelationshipsByIdentityResponse() { Relationships = new List<Relationship>() });

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => identityDeleter.Delete(A<IdentityAddress>._)).MustHaveHappened(2, Times.Exactly);
    }

    [Fact]
    public async Task Worker_Calls_FindRelationshipsByIdentityCommand_For_Each_Identity()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress3 = TestDataGenerator.CreateRandomIdentityAddress();
        RegisterFindRipeDeletionProcessesCommand(mediator, identityAddress1, identityAddress2, identityAddress3);
        A.CallTo(() => mediator.Send(A<FindRelationshipsByIdentityCommand>._, A<CancellationToken>._)).Returns(new FindRelationshipsByIdentityResponse() { Relationships = new List<Relationship>() });

        var worker = GetWorker(mediator, null, null);

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => mediator.Send(A<FindRelationshipsByIdentityCommand>._, A<CancellationToken>._)).MustHaveHappened(3, Times.Exactly);
    }

    [Fact]
    public async Task Worker_Publishes_PeerIdentityDeletedIntegrationEvent_For_Each_Pair_Identity_Relationship()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();
        RegisterFindRipeDeletionProcessesCommand(mediator, identityAddress1, identityAddress2);

        var eventBus = A.Fake<IEventBus>();
        var worker = GetWorker(mediator, null, eventBus);

        A.CallTo(() =>
            mediator.Send(A<FindRelationshipsByIdentityCommand>.That.Matches(x => x.IdentityAddress == identityAddress1), A<CancellationToken>._)
        ).Returns(
            new FindRelationshipsByIdentityResponse() { Relationships = new List<Relationship>() { CreateRelationship(identityAddress1) } }
        );

        A.CallTo(() =>
            mediator.Send(A<FindRelationshipsByIdentityCommand>.That.Matches(x => x.IdentityAddress == identityAddress2), A<CancellationToken>._)
        ).Returns(
            new FindRelationshipsByIdentityResponse() { Relationships = new List<Relationship>() { CreateRelationship(identityAddress2), CreateRelationship(identityAddress2) } }
        );

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => eventBus.Publish(A<PeerIdentityDeletedIntegrationEvent>.That.Matches(x => x.IdentityAddress == identityAddress1))).MustHaveHappened(1, Times.Exactly);
        A.CallTo(() => eventBus.Publish(A<PeerIdentityDeletedIntegrationEvent>.That.Matches(x => x.IdentityAddress == identityAddress2))).MustHaveHappened(2, Times.Exactly);
    }

    private void RegisterFindRipeDeletionProcessesCommand(IMediator mediator, params string[] identityAddresses)
    {
        var commandResponse = new FindRipeDeletionProcessesResponse
        {
            IdentityAddresses = [.. identityAddresses]
        };

        A.CallTo(() => mediator.Send(A<FindRipeDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);
    }

    private static IdentityDeletionWorker GetWorker(IMediator mediator, List<IIdentityDeleter>? identityDeleters, IEventBus? eventBus)
    {
        identityDeleters ??= new List<IIdentityDeleter>([A.Dummy<IIdentityDeleter>()]);
        return new IdentityDeletionWorker(A.Dummy<IHostApplicationLifetime>(), identityDeleters, mediator, A.Dummy<IPushNotificationSender>(), eventBus ?? A.Dummy<IEventBus>());
    }

    private static Relationship CreateRelationship(IdentityAddress identityAddress2)
    {
        var templateCreator = IdentityAddress.Create(new byte[2], "id0");
        var template = new RelationshipTemplate(templateCreator, DeviceId.New(), 1, SystemTime.UtcNow, new byte[2]);

        return new(template, identityAddress2, template.CreatedByDevice, TestDataGenerator.CreateRandomBytes());
    }
}
