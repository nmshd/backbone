using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Module.Features.DomainEvents.DatawalletModificationCreated;
using Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.Datawallet;
using Backbone.Modules.Synchronization.Contracts.DomainEvents;
using FakeItEasy;

namespace Backbone.Modules.Devices.Module.Tests.Tests.PushNotifications.DatawalletModified;

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
        A.CallTo(() => mockSender.SendNotification(A<DatawalletModificationsCreatedPushNotification>._,
                A<SendPushNotificationFilter>.That.Matches(f =>
                    f.IncludedIdentities.Contains(identity) &&
                    f.ExcludedDevices.Contains(modifiedByDevice)),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
