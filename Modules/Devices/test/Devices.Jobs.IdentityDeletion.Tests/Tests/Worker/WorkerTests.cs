using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using FakeItEasy;
using MediatR;
using Xunit;
using static Backbone.Modules.Devices.Jobs.IdentityDeletion.Worker;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion.Tests.Tests.Worker;
public class WorkerTests
{
    [Fact]
    public async Task Worker_Call_Command_To_Get_Identities()
    {
        // Arrange
        var mediator = A.Fake<IMediator>();
        var commandResponse = new UpdateDeletionProcessesResponse
        {
            IdentityAddresses = new List<string>()
        };

        A.CallTo(() => mediator.Send(A<UpdateDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(commandResponse);
        var identityDeleters = new List<IIdentityDeleter>();

        // Act
        await StartProcessing(mediator, identityDeleters, CancellationToken.None);

        // Assert
        A.CallTo(() => mediator.Send(A<UpdateDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
