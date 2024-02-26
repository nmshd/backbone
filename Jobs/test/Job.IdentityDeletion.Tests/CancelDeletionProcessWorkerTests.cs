using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using MediatR;

namespace Backbone.Job.IdentityDeletion.Tests
{
    public class CancelDeletionProcessWorkerTests
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            _ = A.Fake<IMediator>();

            //var commandResponse = new TriggerOldDeletionProcessesResponse
            //{
            //    WaitingOnAppruvalAddress = [.. identityAddresses]
            //};
            //A.CallTo(() => mockMediator.Send(A<TriggerOldDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);

            //// Act
            //await CancelDeletionProcessWorker.StartAsync(CancellationToken.None);

            //// Assert
            //A.CallTo(() => mockMediator.Send(A<TriggerOldDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}
