using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleDeletionProcesses;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.UnitTestTools.Data;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backbone.Job.IdentityDeletion.Tests
{
    public class CancelDeletionProcessWorkerTests
    {
        [Fact]
        public async void Correct_handler_is_triggered()
        {
            // Arrange
            var mockMediator = A.Fake<IMediator>();

            var cancelDeletionProcessWorker = new CancelDeletionProcessWorker(
                A.Dummy<IHostApplicationLifetime>(),
                mockMediator,
                A.Dummy<IPushNotificationSender>(),
                A.Dummy<IEventBus>(),
                A.Dummy<ILogger<CancelDeletionProcessWorker>>());

            // Act
            await cancelDeletionProcessWorker.StartAsync(CancellationToken.None);

            // Assert
            A.CallTo(() => mockMediator.Send(A<CancelStaleDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void Cancel_deletion_process_with_past_due_approval()
        {
            // Arrange
            var mockMediator = A.Fake<IMediator>();

            var identity = new Identity("", IdentityAddress.Create([], "id1"), [], TierId.Generate(), 1);

            var identityDeletion = identity.StartDeletionProcessAsSupport();
            var identityDeleters = new CancelStaleDeletionProcessesResponse();
            identityDeleters.IdentityDeletionProcesses.Add(identity);

            var cancelDeletionProcessWorker = new CancelDeletionProcessWorker(
                A.Dummy<IHostApplicationLifetime>(),
                mockMediator,
                A.Dummy<IPushNotificationSender>(),
                A.Dummy<IEventBus>(),
                A.Dummy<ILogger<CancelDeletionProcessWorker>>());

            A.CallTo(() => mockMediator.Send(A<CancelStaleDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(identityDeleters);

            // Act
            await cancelDeletionProcessWorker.StartAsync(CancellationToken.None);

            // Assert
            identityDeletion.Status.Should().Be(DeletionProcessStatus.Canceled);
        }

        [Fact]
        public async void Publish_deletion_process_canceled_IntegrationEvent()
        {
            // Arrange
            var mockMediator = A.Fake<IMediator>();
            var mockEventBus = A.Fake<IEventBus>();

            var identity = new Identity(TestDataGenerator.CreateRandomIdentityAddress(), IdentityAddress.Create([], "id1"), [], TierId.Generate(), 1);

            var identityDeletion = identity.StartDeletionProcessAsSupport();
            var identityDeleters = new CancelStaleDeletionProcessesResponse();
            identityDeleters.IdentityDeletionProcesses.Add(identity);

            var cancelDeletionProcessWorker = new CancelDeletionProcessWorker(
                A.Dummy<IHostApplicationLifetime>(),
                mockMediator,
                A.Dummy<IPushNotificationSender>(),
                mockEventBus,
                A.Dummy<ILogger<CancelDeletionProcessWorker>>());

            A.CallTo(() => mockMediator.Send(A<CancelStaleDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(identityDeleters);

            // Act
            await cancelDeletionProcessWorker.StartAsync(CancellationToken.None);

            // Assert
            A.CallTo(() =>
                mockEventBus.Publish(A<IdentityDeletionProcessStatusChangedIntegrationEvent>
                    .That.Matches(x => x.Address == identity.Address
                    && (string)x.DeletionProcessId == identityDeletion.Id)))
                .MustHaveHappenedOnceExactly();
        }

        //public static Identity CreateIdentityWithDeletionProcessWaitingForApproval(DateTime deletionProcessStartedAt)
        //{
        //    var currentDateTime = SystemTime.UtcNow;

        //    var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        //    var identity = new Identity(identityAddress, IdentityAddress.Create([], "id1"), [], TierId.Generate(), 1);
        //    SystemTime.Set(deletionProcessStartedAt);
        //    identity.StartDeletionProcessAsSupport();

        //    SystemTime.Set(currentDateTime);

        //    return identity;
        //}
    }
}
