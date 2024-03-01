using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelStaleDeletionProcesses;
public class HandlerTests
{
    [Fact]
    public async Task Empty_list_is_returned_if_no_deletion_process_approvals_are_past_due()
    {
        // Arrange
        var handler = new Handler(A.Fake<IIdentitiesRepository>(), A.Fake<IEventBus>());

        // Act
        var response = await handler.Handle(new CancelStaleDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.StaleDeletionPrecessIdentities.Count.Should().Be(0);
    }

    [Fact]
    public async Task Only_correct_deletion_processes_are_canceled()
    {
        // Arrange
        var identityWithDeletionProcess = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.UtcNow);
        var identityWithStaleDeletionProcess = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.UtcNow.AddDays(-11));

        var identities = new List<Identity>() { identityWithStaleDeletionProcess, identityWithDeletionProcess };

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.FindAllWithDeletionProcessInStatus(A<DeletionProcessStatus>._, A<CancellationToken>._, A<bool>._))
            .Returns(identities);

        var handler = new Handler(fakeIdentitiesRepository, A.Fake<IEventBus>());

        // Act
        var response = await handler.Handle(new CancelStaleDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.StaleDeletionPrecessIdentities.Count.Should().Be(1);
        response.StaleDeletionPrecessIdentities[0].Address.Should().Be(identityWithStaleDeletionProcess.Address);
    }

    //[Fact]
    //public async Task Publish_IntegrationEvent_for_canceled_deletion_process()
    //{
    //    // Arrange
    //    var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.UtcNow.AddDays(-11));
    //    var identities = new List<Identity>() { identity };

    //    var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
    //    var mockEventBus = A.Fake<IEventBus>();

    //    A.CallTo(() => fakeIdentitiesRepository.FindAllWithDeletionProcessInStatus(A<DeletionProcessStatus>._, A<CancellationToken>._, A<bool>._))
    //        .Returns(identities);

    //    var handler = new Handler(fakeIdentitiesRepository, mockEventBus);

    //    // Act
    //    await handler.Handle(new CancelStaleDeletionProcessesCommand(), CancellationToken.None);

    //    // Assert
    //    A.CallTo(() => mockEventBus.Publish(A<IdentityDeletionProcessStatusChangedIntegrationEvent>.That.Matches(i =>
    //            i.Address == identity.Address &&
    //            (string)i.DeletionProcessId == identity.DeletionProcesses[0].Id)))
    //        .MustHaveHappenedOnceExactly();
    //}
}
