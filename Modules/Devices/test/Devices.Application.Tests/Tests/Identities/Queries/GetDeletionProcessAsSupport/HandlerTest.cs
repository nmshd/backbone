using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tests;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using FakeItEasy.Sdk;
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
        //response.CreatedAt.Should().Be(identity.CreatedAt);
        response.ApprovalReminder1SentAt.Should().Be(null);
        response.ApprovalReminder2SentAt.Should().Be(null);
        response.ApprovalReminder3SentAt.Should().Be(null);
        response.ApprovedAt.Should().Be(deletionProcess.ApprovedAt);
        response.ApprovedByDevice.Should().Be(identity.Devices[0].Id);
        response.GracePeriodEndsAt.Should().Be(identity.DeletionGracePeriodEndsAt);
        response.GracePeriodReminder1SentAt.Should().Be(null);
        response.GracePeriodReminder2SentAt.Should().Be(null);
        response.GracePeriodReminder3SentAt.Should().Be(null);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}
