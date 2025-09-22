using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Job.IdentityDeletion.Workers;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsOfIdentity;
using CSharpFunctionalExtensions;
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backbone.Job.IdentityDeletion.Tests.Tests;

public class ActualDeletionWorkerTests : AbstractTestsBase
{
    [Fact]
    public async Task Proxies_triggering_the_deletion_processes_to_command_handler()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        SetupRipeDeletionProcessesCommand(mockMediator);

        var worker = CreateWorker(mockMediator, []);

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => mockMediator.Send(A<TriggerRipeDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Calls_Deleters_For_Each_Identity()
    {
        // Arrange
        var fakeMediator = A.Fake<IMediator>();
        var identity1 = CreateIdentity();
        var identity2 = CreateIdentity();
        SetupRipeDeletionProcessesCommand(fakeMediator, identity1.Address, identity2.Address);

        var mockIdentityDeleter = A.Fake<IIdentityDeleter>();
        var worker = CreateWorker(fakeMediator, [mockIdentityDeleter]);

        A.CallTo(() => fakeMediator.Send(A<ListRelationshipsOfIdentityQuery>._, A<CancellationToken>._))
            .Returns(new ListRelationshipsOfIdentityResponse([]));

        A.CallTo(() => fakeMediator.Send(A<GetIdentityQuery>.That.Matches(q => q.Address == identity1.Address.Value), A<CancellationToken>._))
            .Returns(new GetIdentityResponse(identity1));

        A.CallTo(() => fakeMediator.Send(A<GetIdentityQuery>.That.Matches(q => q.Address == identity2.Address.Value), A<CancellationToken>._))
            .Returns(new GetIdentityResponse(identity2));

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentityDeleter.Delete(identity1.Address)).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentityDeleter.Delete(identity2.Address)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Sends_push_notification_to_each_deleted_identity()
    {
        // Arrange
        var fakeMediator = A.Fake<IMediator>();
        var identity1 = CreateIdentity();
        var identity2 = CreateIdentity();
        SetupRipeDeletionProcessesCommand(fakeMediator, identity1.Address, identity2.Address);
        A.CallTo(() => fakeMediator.Send(A<ListRelationshipsOfIdentityQuery>._, A<CancellationToken>._)).Returns(new ListRelationshipsOfIdentityResponse([]));

        A.CallTo(() => fakeMediator.Send(A<GetIdentityQuery>.That.Matches(q => q.Address == identity1.Address.Value), A<CancellationToken>._))
            .Returns(new GetIdentityResponse(identity1));

        A.CallTo(() => fakeMediator.Send(A<GetIdentityQuery>.That.Matches(q => q.Address == identity2.Address.Value), A<CancellationToken>._))
            .Returns(new GetIdentityResponse(identity2));

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var worker = CreateWorker(fakeMediator, [], mockPushNotificationSender);

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        foreach (var identityAddress in new[] { identity1.Address, identity2.Address })
        {
            A.CallTo(() => mockPushNotificationSender.SendNotification(
                A<DeletionStartsPushNotification>._,
                A<SendPushNotificationFilter>.That.Matches(f => f.IncludedIdentities.Contains(identityAddress)),
                A<CancellationToken>._)
            ).MustHaveHappenedOnceExactly();
        }
    }

    private static void SetupRipeDeletionProcessesCommand(IMediator mediator, params IdentityAddress[] identityAddresses)
    {
        var commandResponse = new TriggerRipeDeletionProcessesResponse(identityAddresses.ToDictionary(x => x.Value, _ => UnitResult.Success<DomainError>()));
        A.CallTo(() => mediator.Send(A<TriggerRipeDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);
    }

    private static ActualDeletionWorker CreateWorker(IMediator mediator,
        List<IIdentityDeleter>? identityDeleters = null,
        IPushNotificationSender? pushNotificationSender = null)
    {
        var hostApplicationLifetime = A.Dummy<IHostApplicationLifetime>();
        identityDeleters ??= [A.Dummy<IIdentityDeleter>()];
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();
        var logger = A.Dummy<ILogger<ActualDeletionWorker>>();
        return new ActualDeletionWorker(hostApplicationLifetime, identityDeleters, mediator, pushNotificationSender, logger);
    }

    private static Identity CreateIdentity()
    {
        return new Identity(
            CreateRandomDeviceId(),
            CreateRandomIdentityAddress(),
            CreateRandomBytes(),
            TierId.Generate(),
            1,
            CommunicationLanguage.DEFAULT_LANGUAGE);
    }
}
