using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.UnitTestTools.Data;
using CSharpFunctionalExtensions;
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Job.IdentityDeletion.Tests.Tests;
public class CancelIdentityDeletionProcessWorkerTests
{
    [Fact]
    public async Task Proxies_triggering_the_deletion_processes_to_command_handler()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();

        SetupRipeDeletionProcessesCommand(mockMediator);

        var identityDeleters = new List<IIdentityDeleter>();
        var worker = CreateWorker(mockMediator, identityDeleters);

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
        SetupRipeDeletionProcessesCommand(mediator, identityAddress1, identityAddress2);
        var mockIdentityDeleter = A.Fake<IIdentityDeleter>();
        var identityDeleters = new List<IIdentityDeleter>([mockIdentityDeleter]);

        var worker = CreateWorker(mediator, identityDeleters);

        A.CallTo(() => mediator.Send(A<FindRelationshipsOfIdentityQuery>._, A<CancellationToken>._)).Returns(new FindRelationshipsOfIdentityResponse(new List<Relationship>()));

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert

        A.CallTo(() => mockIdentityDeleter.Delete(identityAddress1)).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentityDeleter.Delete(identityAddress2)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Sends_PushNotification_For_Each_Relationship_In_Each_Identity()
    {
        // Arrange
        var mockMediator = A.Fake<IMediator>();
        var identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress3 = TestDataGenerator.CreateRandomIdentityAddress();
        SetupRipeDeletionProcessesCommand(mockMediator, identityAddress1, identityAddress2, identityAddress3);
        A.CallTo(() => mockMediator.Send(A<FindRelationshipsOfIdentityQuery>._, A<CancellationToken>._)).Returns(new FindRelationshipsOfIdentityResponse(new List<Relationship>()));

        var pushNotificationSender = A.Dummy<IPushNotificationSender>();
        var worker = CreateWorker(mockMediator, pushNotificationSender: pushNotificationSender);

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        foreach (var identityAddress in new[] { identityAddress1, identityAddress2, identityAddress3 })
        {
            A.CallTo(() => pushNotificationSender.SendNotification(identityAddress, A<object>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }

    private void SetupRipeDeletionProcessesCommand(IMediator mediator, params IdentityAddress[] identityAddresses)
    {
        var commandResponse = new TriggerRipeDeletionProcessesResponse(identityAddresses.ToDictionary(x => x, _ => UnitResult.Success<DomainError>()));
        A.CallTo(() => mediator.Send(A<TriggerRipeDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);
    }

    private static ActualIdentityDeletionWorker CreateWorker(IMediator mediator,
                                       List<IIdentityDeleter>? identityDeleters = null,
                                       IEventBus? eventBus = null,
                                       IPushNotificationSender? pushNotificationSender = null)
    {
        var hostApplicationLifetime = A.Dummy<IHostApplicationLifetime>();
        identityDeleters ??= [A.Dummy<IIdentityDeleter>()];
        eventBus ??= A.Dummy<IEventBus>();
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();
        var logger = A.Dummy<ILogger<ActualIdentityDeletionWorker>>();
        return new ActualIdentityDeletionWorker(hostApplicationLifetime, identityDeleters, mediator, pushNotificationSender, eventBus, logger);
    }
}
