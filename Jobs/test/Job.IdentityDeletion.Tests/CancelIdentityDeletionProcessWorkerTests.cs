using Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleDeletionProcesses;
using FakeItEasy;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backbone.Job.IdentityDeletion.Tests
{
    public class CancelIdentityDeletionProcessWorkerTests
    {
        [Fact]
        public async void Happy_path()
        {
            // Arrange
            var mockMediator = A.Fake<IMediator>();

            var cancelDeletionProcessWorker = CreateWorker(mockMediator);

            // Act
            await cancelDeletionProcessWorker.StartAsync(CancellationToken.None);

            // Assert
            A.CallTo(() => mockMediator.Send(A<CancelStaleDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        private static CancelIdentityDeletionProcessWorker CreateWorker(IMediator? mediator = null)
        {
            mediator ??= A.Fake<IMediator>();

            return new CancelIdentityDeletionProcessWorker(
                A.Dummy<IHostApplicationLifetime>(),
                mediator,
                A.Dummy<ILogger<CancelIdentityDeletionProcessWorker>>());
        }
    }
}
