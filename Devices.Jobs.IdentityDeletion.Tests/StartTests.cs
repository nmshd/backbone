using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using Backbone.Modules.Devices.Jobs.IdentityDeletion;
using FakeItEasy;
using Xunit;

namespace Backbone.Devices.Jobs.IdentityDeletion.Tests;

public class StartTests
{
    [Fact]
    public async Task Worker_Calls_UpdateDeletionProcessesCommand()
    {
        // Arrange
        var mediator = A.Fake<MediatR.IMediator>();
        A.CallTo(() => mediator.Send(A<UpdateDeletionProcessesCommand>._, A<CancellationToken>._)).Returns(new UpdateDeletionProcessesResponse() { IdentityAddresses = [] });

        // Act
        await Worker.StartProcessing(mediator, CancellationToken.None);

        // Assert
        A.CallTo(() => mediator.Send(A<UpdateDeletionProcessesCommand>._, A<CancellationToken>._)).MustHaveHappened();
    }
}

