using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetDeletionProcessAsSupport;
public class HandlerTest
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identity.Address, CancellationToken.None, A<bool>._)).Returns(identity);

        var handler = CreateHandler(mockIdentitiesRepository);
        var query = new GetDeletionProcessAsSupportQuery(identity.Address, deletionProcess.Id);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Status.Should().Be(DeletionProcessStatus.Approved);
        response.ApprovalReminder1SentAt.Should().Be(deletionProcess.ApprovalReminder1SentAt);
        response.ApprovalReminder2SentAt.Should().Be(deletionProcess.ApprovalReminder2SentAt);
        response.ApprovalReminder3SentAt.Should().Be(deletionProcess.ApprovalReminder3SentAt);
        response.ApprovedAt.Should().Be(deletionProcess.ApprovedAt);
        response.ApprovedByDevice.Should().Be(identity.Devices[0].Id);
        response.GracePeriodEndsAt.Should().Be(identity.DeletionGracePeriodEndsAt);
        response.GracePeriodReminder1SentAt.Should().Be(deletionProcess.GracePeriodReminder1SentAt);
        response.GracePeriodReminder2SentAt.Should().Be(deletionProcess.GracePeriodReminder2SentAt);
        response.GracePeriodReminder3SentAt.Should().Be(deletionProcess.GracePeriodReminder3SentAt);
    }

    [Fact]
    public async Task Throws_when_no_identity_found()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._)).Returns<Identity?>(null);

        var handler = CreateHandler(mockIdentitiesRepository);
        var query = new GetDeletionProcessAsSupportQuery("some-inexistent-identity-address", "some-inexistent-deletion-process-id");

        // Act
        Func<Task> acting = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Throws_when_no_deletion_process_found()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identity.Address, CancellationToken.None, A<bool>._)).Returns(identity);

        var handler = CreateHandler(mockIdentitiesRepository);
        var query = new GetDeletionProcessAsSupportQuery(identity.Address, "some-inexistent-deletion-process-id");

        // Assert
        Func<Task> acting = async () => await handler.Handle(query, CancellationToken.None);

        // Act
        await acting.Should().ThrowAsync<NotFoundException>();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}
