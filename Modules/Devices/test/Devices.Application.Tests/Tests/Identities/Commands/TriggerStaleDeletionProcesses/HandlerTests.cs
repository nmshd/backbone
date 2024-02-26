using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerStaleDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.TriggerStaleDeletionProcesses;
public class HandlerTests
{
    [Fact]
    public async Task Happy_Path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.UtcNow.AddDays(-11));
        var deletionProcess = activeIdentity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(activeIdentity.Address, A<CancellationToken>._, A<bool>._)).Returns(activeIdentity);

        var handler = new Handler(mockIdentitiesRepository);

        // Act
        var response = await handler.Handle(new TriggerStaleDeletionProcessesCommand(activeIdentity.Address, deletionProcess.Id), CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(DeletionProcessStatus.Canceled);

        A.CallTo(() => mockIdentitiesRepository.Update(
                A<Identity>.That.Matches(
                    i => i.Address == activeIdentity.Address &&
                         i.DeletionProcesses.Count == 1 &&
                         i.DeletionProcesses[0].Id == response.Id),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
