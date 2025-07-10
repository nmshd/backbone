using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsOwner;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.CancelDeletionProcessAsOwner;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess();
        var activeDevice = activeIdentity.Devices[0];
        var deletionProcess = activeIdentity.GetDeletionProcessInStatus(DeletionProcessStatus.Approved)!;

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.Get(activeIdentity.Address, CancellationToken.None, A<bool>._))
            .Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(activeIdentity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice.Id);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext, mockPushNotificationSender);

        // Act
        var response = await handler.Handle(new CancelDeletionProcessAsOwnerCommand { DeletionProcessId = deletionProcess.Id }, CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == activeIdentity.Address
                && i.Status == IdentityStatus.Active
                && i.DeletionProcesses[0].Status == DeletionProcessStatus.Cancelled
                && i.DeletionProcesses[0].Id == deletionProcess.Id), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        response.Status.ShouldBe(DeletionProcessStatus.Cancelled);

        A.CallTo(() => mockPushNotificationSender.SendNotification(
            A<DeletionProcessCancelledByOwnerPushNotification>._,
            A<SendPushNotificationFilter>.That.Matches(f => f.IncludedIdentities.Contains(activeIdentity.Address)),
            A<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();
        var handler = CreateHandler();

        // Act
        var acting = async () => await handler.Handle(new CancelDeletionProcessAsOwnerCommand { DeletionProcessId = address }, CancellationToken.None);

        // Assert
        acting.ShouldThrowAsync<NotFoundException>();
    }

    private static Handler CreateHandler(IIdentitiesRepository? identitiesRepository = null, IUserContext? userContext = null, IPushNotificationSender? pushNotificationSender = null)
    {
        userContext ??= A.Dummy<IUserContext>();
        identitiesRepository ??= A.Dummy<IIdentitiesRepository>();
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();

        return new Handler(identitiesRepository, userContext, pushNotificationSender);
    }
}
