using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleIdentityDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelStaleIdentityDeletionProcesses;

public class HandlerTests
{
    [Fact]
    public async Task Empty_list_is_returned_if_no_deletion_process_approvals_are_past_due()
    {
        // Arrange
        var handler = new Handler(A.Fake<IIdentitiesRepository>(), A.Fake<IEventBus>());

        // Act
        var response = await handler.Handle(new CancelStaleIdentityDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeEmpty();
    }

    [Fact]
    public async Task Only_correct_deletion_processes_are_cancelled()
    {
        // Arrange
        var identityWithDeletionProcess = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(deletionProcessStartedAt: SystemTime.UtcNow);
        var identityWithStaleDeletionProcess = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(deletionProcessStartedAt: SystemTime.UtcNow.AddDays(-11));

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.FindAllWithDeletionProcessInStatus(A<DeletionProcessStatus>._, A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity>() { identityWithStaleDeletionProcess, identityWithDeletionProcess });

        var handler = new Handler(fakeIdentitiesRepository, A.Fake<IEventBus>());

        // Act
        var response = await handler.Handle(new CancelStaleIdentityDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Should().HaveCount(1);
        response.First().Should().Be(identityWithStaleDeletionProcess.DeletionProcesses[0].Id);
    }

    [Fact]
    public async Task Publishes_IntegrationEvent_for_cancelled_deletion_process()
    {
        // Arrange
        var identity1 = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(SystemTime.UtcNow.AddDays(-11));
        var identity2 = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(SystemTime.UtcNow.AddDays(-11));

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockEventBus = A.Fake<IEventBus>();

        A.CallTo(() => fakeIdentitiesRepository.FindAllWithDeletionProcessInStatus(A<DeletionProcessStatus>._, A<CancellationToken>._, A<bool>._))
            .Returns(new List<Identity>() { identity1, identity2 });

        var handler = new Handler(fakeIdentitiesRepository, mockEventBus);

        // Act
        await handler.Handle(new CancelStaleIdentityDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        A.CallTo(() => mockEventBus.Publish(A<IdentityDeletionProcessStatusChangedIntegrationEvent>.That.Matches(i =>
                i.Address == identity1.Address &&
                i.DeletionProcessId == identity1.DeletionProcesses[0].Id)))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockEventBus.Publish(A<IdentityDeletionProcessStatusChangedIntegrationEvent>.That.Matches(i =>
                i.Address == identity2.Address &&
                i.DeletionProcessId == identity2.DeletionProcesses[0].Id)))
            .MustHaveHappenedOnceExactly();
    }
}
