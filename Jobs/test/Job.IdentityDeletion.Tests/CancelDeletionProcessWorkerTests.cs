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
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backbone.Job.IdentityDeletion.Tests
{
    public class CancelDeletionProcessWorkerTests
    {
        [Fact]
        public async void Test1()
        {
            // Arrange
            var mockMediator = A.Fake<IMediator>();
            var hostApplicationLifetime = A.Dummy<IHostApplicationLifetime>();

            var activeIdentity = new Identity("", IdentityAddress.Create([], "id1"), [], TierId.Generate(), 1);

            var identityDeletion = activeIdentity.StartDeletionProcessAsSupport();
            var identityDeleters = new List<IdentityDeletionProcess>() { identityDeletion };

            var commandResponse = new TriggerStaleDeletionProcessesResponse(identityDeletion);

            var cancelDeletionProcessWorker = new CancelDeletionProcessWorker(
                hostApplicationLifetime,
                mockMediator,
                A.Dummy<IPushNotificationSender>(),
                A.Dummy<IEventBus>(),
                A.Dummy<ILogger<CancelDeletionProcessWorker>>(),
                identityDeleters);

            A.CallTo(() => mockMediator.Send(A<TriggerStaleDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);

            // Act
            await cancelDeletionProcessWorker.StartAsync(CancellationToken.None);

            // Assert
            A.CallTo(() => mockMediator.Send(A<TriggerStaleDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}
