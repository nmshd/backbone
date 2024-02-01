using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion.Tests.Tests;
public class WorkerTests
{
    [Fact]
    public async Task Proxies_deletion_to_command_handler()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();

        RegisterFindRipeDeletionProcessesCommand(mockMediator);

        var identityDeleters = new List<IIdentityDeleter>();
        var worker = CreateWorker(mockMediator, identityDeleters, null);

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => mockMediator.Send(A<TriggerRipeDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Calls_Deleters_For_Each_Identity()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();
        RegisterFindRipeDeletionProcessesCommand(mediator, identityAddress1, identityAddress2);
        var mockIdentityDeleter = A.Dummy<IIdentityDeleter>();
        var identityDeleters = new List<IIdentityDeleter>([mockIdentityDeleter]);

        var worker = CreateWorker(mediator, identityDeleters, null);

        A.CallTo(() => mediator.Send(A<FindRelationshipsOfIdentityQuery>._, A<CancellationToken>._)).Returns(new FindRelationshipsOfIdentityResponse() { Relationships = new List<Relationship>() });

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert

        A.CallTo(() => mockIdentityDeleter.Delete(identityAddress1)).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentityDeleter.Delete(identityAddress2)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Calls_FindRelationshipsByIdentityCommand_For_Each_Identity()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress3 = TestDataGenerator.CreateRandomIdentityAddress();
        RegisterFindRipeDeletionProcessesCommand(mockMediator, identityAddress1, identityAddress2, identityAddress3);
        A.CallTo(() => mockMediator.Send(A<FindRelationshipsOfIdentityQuery>._, A<CancellationToken>._)).Returns(new FindRelationshipsOfIdentityResponse() { Relationships = new List<Relationship>() });

        var worker = CreateWorker(mockMediator, null, null);

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => mockMediator.Send(A<FindRelationshipsOfIdentityQuery>._, A<CancellationToken>._)).MustHaveHappened(3, Times.Exactly);
    }

    [Fact]
    public async Task Publishes_PeerIdentityDeletedIntegrationEvent_For_Each_Pair_Identity_Relationship()
    {
        // Arrange
        var fakeMediator = A.Fake<IMediator>();
        var identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();
        RegisterFindRipeDeletionProcessesCommand(fakeMediator, identityAddress1, identityAddress2);

        var mockEventBus = A.Fake<IEventBus>();
        var worker = CreateWorker(fakeMediator, mockEventBus);

        A.CallTo(() =>
            fakeMediator.Send(A<FindRelationshipsOfIdentityQuery>.That.Matches(x => x.IdentityAddress == identityAddress1), A<CancellationToken>._)
        ).Returns(
            new FindRelationshipsOfIdentityResponse() { Relationships = new List<Relationship>() { CreateRelationship(identityAddress1) } }
        );

        A.CallTo(() =>
            fakeMediator.Send(A<FindRelationshipsOfIdentityQuery>.That.Matches(x => x.IdentityAddress == identityAddress2), A<CancellationToken>._)
        ).Returns(
            new FindRelationshipsOfIdentityResponse() { Relationships = new List<Relationship>() { CreateRelationship(identityAddress2), CreateRelationship(identityAddress2) } }
        );

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.Publish(A<PeerIdentityDeletedIntegrationEvent>.That.Matches(x => x.IdentityAddress == identityAddress1))).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockEventBus.Publish(A<PeerIdentityDeletedIntegrationEvent>.That.Matches(x => x.IdentityAddress == identityAddress2))).MustHaveHappenedTwiceExactly();
    }

    private void RegisterFindRipeDeletionProcessesCommand(IMediator mediator, params string[] identityAddresses)
    {
        var commandResponse = new TriggerRipeDeletionProcessesResponse
        {
            IdentityAddresses = [.. identityAddresses]
        };

        A.CallTo(() => mediator.Send(A<TriggerRipeDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);
    }

    private static Worker CreateWorker(IMediator mediator, IEventBus? eventBus)
    {
        return CreateWorker(mediator, null, eventBus);
    }

    private static Worker CreateWorker(IMediator mediator, List<IIdentityDeleter>? identityDeleters, IEventBus? eventBus)
    {
        var hostApplicationLifetime = A.Dummy<IHostApplicationLifetime>();
        identityDeleters ??= [A.Dummy<IIdentityDeleter>()];
        eventBus ??= A.Dummy<IEventBus>();
        var pushNotificationSender = A.Dummy<IPushNotificationSender>();
        var logger = A.Dummy<ILogger<Worker>>();
        return new Worker(hostApplicationLifetime, identityDeleters, mediator, pushNotificationSender, eventBus, logger);
    }

    private static Relationship CreateRelationship(IdentityAddress identityAddress)
    {
        var templateCreator = IdentityAddress.Create(new byte[2], "id0");
        var template = new RelationshipTemplate(templateCreator, DeviceId.New(), 1, SystemTime.UtcNow, new byte[2]);

        return new Relationship(template, identityAddress, template.CreatedByDevice, TestDataGenerator.CreateRandomBytes());
    }
}
