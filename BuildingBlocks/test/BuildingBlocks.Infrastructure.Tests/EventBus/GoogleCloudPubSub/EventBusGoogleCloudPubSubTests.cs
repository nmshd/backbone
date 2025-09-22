using Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Backbone.Modules.TestModule.Application.DomainEvents;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub;

public class EventBusGoogleCloudPubSubTests : AbstractTestsBase
{
    [Fact]
    public void GetSubscriptionName()
    {
        // Act
        var subscriptionName = EventBusGoogleCloudPubSub.GetSubscriptionName<TestDomainEventHandler, TestDomainEvent>("testProject");

        // Assert
        subscriptionName.ProjectId.ShouldBe("testProject");
        subscriptionName.SubscriptionId.ShouldBe("TestModule.Test");
    }
}
