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
    public async Task Happy_Path()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.UtcNow.AddDays(-11));
        var identities = new List<Identity>() { identity };        

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, A<CancellationToken>._, A<bool>._))
            .Returns(identities);

        var handler = new Handler(mockIdentitiesRepository);

        // Act
        var response = await handler.Handle(new CancelStaleDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.IdentityDeletionProcesses.Count.Should().Be(1);
        response.IdentityDeletionProcesses[0].Address.Should().Be(identity.Address);
    }

    [Fact]
    public async Task Empty_list_is_returned_if_no_deletion_process_approvals_are_past_due()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        var handler = new Handler(mockIdentitiesRepository);

        // Act
        var response = await handler.Handle(new CancelStaleDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.IdentityDeletionProcesses.Count.Should().Be(0);
    }

    [Fact]
    public async Task Only_identities_with_correct_deletion_process_are_handled() // todo: rename ??? only correct deletion processes are canceled
    {
        // Arrange
        var mockIdentity = TestDataGenerator.CreateIdentity();
        var identityWithDeletionProcess = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.UtcNow);

        var identityWithStaleDeletionProcess = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.UtcNow.AddDays(-11));

        var identities = new List<Identity>() { identityWithStaleDeletionProcess, mockIdentity, identityWithDeletionProcess };

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, A<CancellationToken>._, A<bool>._))
            .Returns(identities);

        var handler = new Handler(mockIdentitiesRepository);

        // Act
        var response = await handler.Handle(new CancelStaleDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.IdentityDeletionProcesses.Count.Should().Be(1);
        response.IdentityDeletionProcesses[0].Address.Should().Be(identityWithStaleDeletionProcess.Address);
    }
}
