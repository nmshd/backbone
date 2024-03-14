using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAsSupport;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetDeletionProcessesAsSupport;
public class HandlerTests
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;

        identity.CancelDeletionProcess(deletionProcess.Id, identity.Devices[0].Id);
        identity.StartDeletionProcessAsOwner(identity.Devices[0].Id);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identity.Address, CancellationToken.None, A<bool>._)).Returns(identity);

        var handler = CreateHandler(mockIdentitiesRepository);
        var query = new GetDeletionProcessesAsSupportQuery(identity.Address);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Count().Should().Be(2);
        AssertDeletionProcessValues(response, identity, 0);
        AssertDeletionProcessValues(response, identity, 1);
    }

    [Fact]
    public async Task Throws_when_no_identity_found()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._)).Returns<Identity?>(null);

        var handler = CreateHandler(mockIdentitiesRepository);
        var query = new GetDeletionProcessesAsSupportQuery("some-inexistent-identity-address");

        // Act
        Func<Task> acting = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<NotFoundException>().WithMessage("*Identity*");
    }

    private static void AssertDeletionProcessValues(GetDeletionProcessesAsSupportResponse response, Identity identity, int index)
    {
        response.ElementAt(index).Status.Should().Be(identity.DeletionProcesses[index].Status);
        response.ElementAt(index).ApprovalReminder1SentAt.Should().Be(identity.DeletionProcesses[index].ApprovalReminder1SentAt);
        response.ElementAt(index).ApprovalReminder2SentAt.Should().Be(identity.DeletionProcesses[index].ApprovalReminder2SentAt);
        response.ElementAt(index).ApprovalReminder3SentAt.Should().Be(identity.DeletionProcesses[index].ApprovalReminder3SentAt);
        response.ElementAt(index).ApprovedAt.Should().Be(identity.DeletionProcesses[index].ApprovedAt);
        response.ElementAt(index).ApprovedByDevice.Should().Be(identity.Devices[0].Id);
        response.ElementAt(index).GracePeriodEndsAt.Should().Be(identity.DeletionGracePeriodEndsAt);
        response.ElementAt(index).GracePeriodReminder1SentAt.Should().Be(identity.DeletionProcesses[index].GracePeriodReminder1SentAt);
        response.ElementAt(index).GracePeriodReminder2SentAt.Should().Be(identity.DeletionProcesses[index].GracePeriodReminder2SentAt);
        response.ElementAt(index).GracePeriodReminder3SentAt.Should().Be(identity.DeletionProcesses[index].GracePeriodReminder3SentAt);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}
