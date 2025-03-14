﻿using Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.RabbitMq;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub;

public class EventBusGoogleCloudPubSubTests : AbstractTestsBase
{
    [Fact]
    public void GetSubscriptionName()
    {
        // Act
        var subscriptionName = EventBusGoogleCloudPubSub.GetSubscriptionName<TestDomainEventHandler, TestDomainEvent>("testProject");

        // Assert
        subscriptionName.ProjectId.Should().Be("testProject");
        subscriptionName.SubscriptionId.Should().Be("TestModule.Test");
    }
}
