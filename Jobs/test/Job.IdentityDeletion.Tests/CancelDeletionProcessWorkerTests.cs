using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerStaleDeletionProcesses;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
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
            A.CallTo(() => mockMediator.Send(A<TriggerStaleDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact(Skip = "next step")]
        public async void Test1()
        {
            // Arrange
            var mockMediator = A.Fake<IMediator>();
            var hostApplicationLifetime = A.Dummy<IHostApplicationLifetime>();

            var activeIdentity = new Identity("", IdentityAddress.Create([], "id1"), [], TierId.Generate(), 1);

            var identityDeletion = activeIdentity.StartDeletionProcessAsSupport();
            var identityDeleters = new List<IdentityDeletionProcess>() { identityDeletion };

            var commandResponse = new TriggerStaleDeletionProcessesResponse();

            var cancelDeletionProcessWorker = new CancelDeletionProcessWorker(
                hostApplicationLifetime,
                mockMediator,
                A.Dummy<IPushNotificationSender>(),
                A.Dummy<IEventBus>(),
                A.Dummy<ILogger<CancelDeletionProcessWorker>>()
                //identityDeleters
                );

            A.CallTo(() => mockMediator.Send(A<TriggerStaleDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);

            // Act
            await cancelDeletionProcessWorker.StartAsync(CancellationToken.None);

            // Assert
            A.CallTo(() => mockMediator.Send(A<TriggerStaleDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Fact(Skip = "next step")]
        public async void Cancels_the_deletion_process_that_is_past_due_approval()
        {
            // Arrange
            var mockMediator = A.Fake<IMediator>();
            var hostApplicationLifetime = A.Dummy<IHostApplicationLifetime>();

            var activeIdentity = new Identity("", IdentityAddress.Create([], "id1"), [], TierId.Generate(), 1);

            var identityDeletion = activeIdentity.StartDeletionProcessAsSupport();
            var identityDeleters = new List<IdentityDeletionProcess>() { identityDeletion };

            var commandResponse = new TriggerStaleDeletionProcessesResponse();

            var cancelDeletionProcessWorker = new CancelDeletionProcessWorker(
                hostApplicationLifetime,
                mockMediator,
                A.Dummy<IPushNotificationSender>(),
                A.Dummy<IEventBus>(),
                A.Dummy<ILogger<CancelDeletionProcessWorker>>()
                //identityDeleters
                );

            A.CallTo(() => mockMediator.Send(A<TriggerStaleDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);

            SystemTime.Set(DateTime.UtcNow.AddDays(11));

            // Act
            await cancelDeletionProcessWorker.StartAsync(CancellationToken.None);

            // Assert
            A.CallTo(() => mockMediator.Send(A<TriggerStaleDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            identityDeletion.Status.Should().Be(DeletionProcessStatus.Canceled);
            activeIdentity.DeletionProcesses.Count.Should().Be(1);
            activeIdentity.Status.Should().Be(IdentityStatus.Active);
        }
    }
}
