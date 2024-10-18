using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.DatawalletModificationCreated;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications.DatawalletModified;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creating_a_Datawallet_modification_sends_a_filtered_notification()
    {
        // Arrange
        var modifiedByDevice = CreateRandomDeviceId();
        var identity = CreateRandomIdentityAddress();
        var mockSender = A.Fake<IPushNotificationSender>();
        var handler = new DatawalletModifiedDomainEventHandler(mockSender);
        var domainEvent = new DatawalletModifiedDomainEvent { Identity = identity, ModifiedByDevice = modifiedByDevice };

        // Act
        await handler.Handle(domainEvent);

        // Assert
        var expectedFilteredDeviceIds = new List<string>([modifiedByDevice.Value]);
        A.CallTo(() => mockSender.SendFilteredNotification(identity, A<IPushNotification>._, expectedFilteredDeviceIds, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
