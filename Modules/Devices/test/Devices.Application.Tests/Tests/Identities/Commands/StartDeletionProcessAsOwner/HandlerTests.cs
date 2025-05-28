using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsOwner;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.StartDeletionProcessAsOwner;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithOneDevice();
        var activeDevice = activeIdentity.Devices[0];

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();
        var mockPushNotificationSender = A.Dummy<IPushNotificationSender>();

        A.CallTo(() => mockIdentitiesRepository.Get(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._))
            .Returns(activeIdentity);
        A.CallTo(() => fakeUserContext.GetAddressOrNull()).Returns(activeIdentity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice.Id);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext, mockPushNotificationSender);

        // Act
        var response = await handler.Handle(new StartDeletionProcessAsOwnerCommand(), CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.ApprovedByDevice.Should().NotBeNull();

        A.CallTo(() => mockIdentitiesRepository.Update(
                A<Identity>.That.Matches(i => i.Address == activeIdentity.Address &&
                                              i.DeletionProcesses.Count == 1 &&
                                              i.DeletionProcesses[0].Id == response.Id &&
                                              i.DeletionProcesses[0].AuditLog.Count == 1),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => mockPushNotificationSender.SendNotification(
            A<DeletionProcessStartedPushNotification>._,
            A<SendPushNotificationFilter>.That.Matches(f => f.IncludedIdentities.Contains(activeIdentity.Address)),
            A<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();

        A.CallTo(() => fakeIdentitiesRepository.Get(
                A<IdentityAddress>._,
                A<CancellationToken>._,
                A<bool>._))
            .Returns<Identity?>(null);
        A.CallTo(() => fakeUserContext.GetAddressOrNull()).Returns(address);

        var handler = CreateHandler(fakeIdentitiesRepository, fakeUserContext);

        // Act
        var acting = async () => await handler.Handle(new StartDeletionProcessAsOwnerCommand(), CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException, StartDeletionProcessAsOwnerResponse>().Which.Message.Should().Contain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IUserContext userContext, IPushNotificationSender? pushNotificationSender = null)
    {
        pushNotificationSender ??= A.Dummy<IPushNotificationSender>();
        return new Handler(identitiesRepository, userContext, pushNotificationSender);
    }
}
