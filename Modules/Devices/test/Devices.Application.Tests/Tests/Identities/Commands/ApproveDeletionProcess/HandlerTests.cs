using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.ApproveDeletionProcess;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var utcNow = DateTime.Parse("2000-01-01");
        SystemTime.Set(utcNow);

        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.Parse("2000-01-10"));
        var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!;
        var device = identity.Devices[0];

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(device.Id);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.Get(identity.Address, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext, mockPushNotificationSender);

        // Act
        var response = await handler.Handle(new ApproveDeletionProcessCommand(deletionProcess.Id), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.Status == IdentityStatus.ToBeDeleted
                && i.TierId == Tier.QUEUED_FOR_DELETION.Id
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.ApprovedAt == DateTime.Parse("2000-01-01")
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.GracePeriodEndsAt == DateTime.Parse("2000-01-15")
                && i.DeletionProcesses.FirstOrDefault(d => d.Status == DeletionProcessStatus.Approved)!.ApprovedByDevice == device.Id
            ), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();


        A.CallTo(() => mockPushNotificationSender.SendNotification(
            A<DeletionProcessApprovedPushNotification>.That.Matches(n => n.DaysUntilDeletion == IdentityDeletionConfiguration.Instance.LengthOfGracePeriodInDays),
            A<SendPushNotificationFilter>.That.Matches(f => f.IncludedIdentities.Contains(identity.Address)),
            A<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();

        response.Id.ShouldBe(deletionProcess.Id);
        response.ApprovedAt.ShouldBe(utcNow);
        response.ApprovedByDevice.ShouldBe(device.Id);
        response.Status.ShouldBe(DeletionProcessStatus.Approved);
    }

    [Fact]
    public async Task Throws_when_given_identity_does_not_exist()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(address);

        A.CallTo(() => fakeIdentitiesRepository.Get(address, A<CancellationToken>._, A<bool>._)).Returns<Identity?>(null);

        var handler = CreateHandler(fakeIdentitiesRepository, fakeUserContext);

        // Act
        var acting = async () => await handler.Handle(new ApproveDeletionProcessCommand("some-deletion-process-id"), CancellationToken.None);

        // Assert
        var exception = await acting.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldContain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IUserContext userContext, IPushNotificationSender? pushNotificationSender = null)
    {
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();
        return new Handler(identitiesRepository, userContext, pushNotificationSender);
    }
}
