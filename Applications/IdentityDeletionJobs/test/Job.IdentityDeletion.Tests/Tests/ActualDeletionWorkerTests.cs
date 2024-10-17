using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Job.IdentityDeletion.Workers;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Relationships.Application.Relationships.Queries.FindRelationshipsOfIdentity;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Data;
using CSharpFunctionalExtensions;
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

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
        var identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();
        SetupRipeDeletionProcessesCommand(fakeMediator, identityAddress1, identityAddress2);

        var identity1 = TestDataGenerator.CreateIdentity(identityAddress1);
        var identity2 = TestDataGenerator.CreateIdentity(identityAddress2);

        var mockIdentityDeleter = A.Fake<IIdentityDeleter>();
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        var worker = CreateWorker(fakeMediator, [mockIdentityDeleter], mockIdentitiesRepository);

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identityAddress1, A<CancellationToken>._, A<bool>._))
            .Returns(identity1);

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identityAddress2, A<CancellationToken>._, A<bool>._))
            .Returns(identity2);

        A.CallTo(() => fakeMediator.Send(A<FindRelationshipsOfIdentityQuery>._, A<CancellationToken>._))
            .Returns(new FindRelationshipsOfIdentityResponse(new List<Relationship>()));

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentityDeleter.Delete(identityAddress1)).MustHaveHappenedOnceExactly();
        A.CallTo(() => mockIdentityDeleter.Delete(identityAddress2)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Sends_push_notification_to_each_deleted_identity()
    {
        // Arrange
        var fakeMediator = A.Fake<IMediator>();
        var identityAddress1 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress2 = TestDataGenerator.CreateRandomIdentityAddress();
        var identityAddress3 = TestDataGenerator.CreateRandomIdentityAddress();
        SetupRipeDeletionProcessesCommand(fakeMediator, identityAddress1, identityAddress2, identityAddress3);
        A.CallTo(() => fakeMediator.Send(A<FindRelationshipsOfIdentityQuery>._, A<CancellationToken>._)).Returns(new FindRelationshipsOfIdentityResponse([]));

        var identity1 = TestDataGenerator.CreateIdentity(identityAddress1);
        var identity2 = TestDataGenerator.CreateIdentity(identityAddress2);
        var identity3 = TestDataGenerator.CreateIdentity(identityAddress3);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identityAddress1, A<CancellationToken>._, A<bool>._))
            .Returns(identity1);

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identityAddress2, A<CancellationToken>._, A<bool>._))
            .Returns(identity2);

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identityAddress3, A<CancellationToken>._, A<bool>._))
            .Returns(identity3);


        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        var worker = CreateWorker(fakeMediator, [], mockIdentitiesRepository);

        // Act
        await worker.StartProcessing(CancellationToken.None);

        // Assert
        foreach (var identityAddress in new[] { identityAddress1, identityAddress2, identityAddress3 })
        {
            A.CallTo(() => mockPushNotificationSender.SendNotification(identityAddress, A<IPushNotification>._, A<CancellationToken>._)).MustNotHaveHappened();
        }
    }

    private static void SetupRipeDeletionProcessesCommand(IMediator mediator, params IdentityAddress[] identityAddresses)
    {
        var commandResponse = new TriggerRipeDeletionProcessesResponse(identityAddresses.ToDictionary(x => x, _ => UnitResult.Success<DomainError>()));
        A.CallTo(() => mediator.Send(A<TriggerRipeDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);
    }

    private static ActualDeletionWorker CreateWorker(IMediator mediator,
        List<IIdentityDeleter>? identityDeleters = null,
        IIdentitiesRepository? identitiesRepository = null,
        IPushNotificationSender? pushNotificationSender = null)
    {
        var hostApplicationLifetime = A.Dummy<IHostApplicationLifetime>();
        identityDeleters ??= [A.Dummy<IIdentityDeleter>()];
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();
        var logger = A.Dummy<ILogger<ActualDeletionWorker>>();
        identitiesRepository ??= A.Dummy<IIdentitiesRepository>();

        return new ActualDeletionWorker(hostApplicationLifetime, identityDeleters, mediator, pushNotificationSender, logger, identitiesRepository);
    }
}
