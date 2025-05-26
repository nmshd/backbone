using Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleIdentityDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelStaleIdentityDeletionProcesses;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Empty_list_is_returned_if_no_deletion_process_approvals_are_past_due()
    {
        // Arrange
        var handler = CreateHandler();

        // Act
        var response = await handler.Handle(new CancelStaleIdentityDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.ShouldNotBeNull();
        response.ShouldBeEmpty();
    }

    [Fact]
    public async Task Only_stale_deletion_processes_are_cancelled()
    {
        // Arrange
        var elevenDaysAgo = DateTime.Parse("2020-01-20");
        var utcNow = DateTime.Parse("2020-01-31");
        SystemTime.Set(utcNow);

        var identityWithDeletionProcess = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(deletionProcessStartedAt: utcNow);
        var identityWithStaleDeletionProcess = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(deletionProcessStartedAt: elevenDaysAgo);

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.FindAllWithDeletionProcessInStatus(A<DeletionProcessStatus>._, A<CancellationToken>._, A<bool>._))
            .Returns([identityWithStaleDeletionProcess, identityWithDeletionProcess]);

        var handler = CreateHandler(fakeIdentitiesRepository);

        // Act
        var response = await handler.Handle(new CancelStaleIdentityDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.ShouldHaveCount(1);
        response.First().ShouldBe(identityWithStaleDeletionProcess.DeletionProcesses[0].Id);
    }

    private static Handler CreateHandler(IIdentitiesRepository? identitiesRepository = null)
    {
        identitiesRepository ??= A.Fake<IIdentitiesRepository>();
        return new Handler(identitiesRepository);
    }
}
