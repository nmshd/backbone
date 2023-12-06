using Backbone.Modules.Devices.Application.Identities.Commands.DeletionProcessGracePeriod;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletionGracePeriod.Tests.Tests;

public class WorkerTests
{
    [Fact]
    public async Task Worker_Calls_Command_To_Process_Grace_Period()
    {
        // Arrange
        var mediator = A.Fake<MediatR.IMediator>();

        // Act
        await Worker.StartProcessing(mediator, CancellationToken.None);

        // Assert
        A.CallTo(() => mediator.Send(A<DeletionProcessGracePeriodCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
