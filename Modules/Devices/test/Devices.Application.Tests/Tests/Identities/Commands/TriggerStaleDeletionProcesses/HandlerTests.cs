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
        var deletionProcess = activeIdentity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!;
        var activeIdentityRepository = new List<Identity>() { activeIdentity };
        

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, A<CancellationToken>._, A<bool>._)).Returns(activeIdentityRepository);

        var handler = new Handler(mockIdentitiesRepository);

        // Act
        var response = await handler.Handle(new TriggerStaleDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.IdentityDeletionProcesses.Count.Should().Be(1);
        response.IdentityDeletionProcesses[0].Status.Should().Be(DeletionProcessStatus.WaitingForApproval);
        response.IdentityDeletionProcesses[0].Id.Should().Be(deletionProcess.Id);
        //response.IdentityDeletionProcesses[0].Status.Should().Be(DeletionProcessStatus.Canceled);

        //A.CallTo(() => mockIdentitiesRepository.Update(
        //        A<Identity>.That.Matches(
        //            i => i.Address == activeIdentity.Address &&
        //                 i.DeletionProcesses.Count == 1 &&
        //                 i.DeletionProcesses[0].Id == response.IdentityDeletionProcesses[0].Id),
        //        A<CancellationToken>._))
        //    .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Test2()
    {
        // Arrange
        var otherIdentity = TestDataGenerator.CreateIdentity();

        var activeIdentity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.UtcNow.AddDays(-11));
        var deletionProcess = activeIdentity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!;
        var activeIdentityRepository = new List<Identity>() { activeIdentity, otherIdentity };


        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, A<CancellationToken>._, A<bool>._))
            .Returns(activeIdentityRepository);

        var handler = new Handler(mockIdentitiesRepository);

        // Act
        var response = await handler.Handle(new TriggerStaleDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.IdentityDeletionProcesses.Count.Should().Be(1);
        response.IdentityDeletionProcesses[0].Status.Should().Be(DeletionProcessStatus.WaitingForApproval);
        response.IdentityDeletionProcesses[0].Id.Should().Be(deletionProcess.Id);
        //response.IdentityDeletionProcesses[0].Status.Should().Be(DeletionProcessStatus.Canceled);

        //A.CallTo(() => mockIdentitiesRepository.Update(
        //        A<Identity>.That.Matches(
        //            i => i.Address == activeIdentity.Address &&
        //                 i.DeletionProcesses.Count == 1 &&
        //                 i.DeletionProcesses[0].Id == response.IdentityDeletionProcesses[0].Id),
        //        A<CancellationToken>._))
        //    .MustHaveHappenedOnceExactly();
    }
}
