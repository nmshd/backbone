using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Devices.Commands.UseBackupDevice;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Devices.Commands.UseBackupDevice;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task A_valid_device_can_be_used()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentity();
        var device = new Device(identity, CommunicationLanguage.DEFAULT_LANGUAGE, null, true);

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => fakeIdentitiesRepository.GetDeviceById(device.Id, A<CancellationToken>._, A<bool>._)).Returns(device);

        var fakePushNotificationsSender = A.Fake<IPushNotificationSender>();
        var handler = new Handler(fakeIdentitiesRepository, fakePushNotificationsSender);

        // Act
        await handler.Handle(new UseBackupDeviceCommand { DeviceId = device.Id }, CancellationToken.None);

        // Assert
        device.IsBackupDevice.Should().BeFalse();
        A.CallTo(() => fakeIdentitiesRepository.Update(A<Device>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => fakePushNotificationsSender.SendNotification(A<IPushNotification>._, A<SendPushNotificationFilter>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
