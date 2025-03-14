﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;
using Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.RabbitMq;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.AzureServiceBus;

public class EventBusAzureServiceBusTests : AbstractTestsBase
{
    [Fact]
    public void GetSubscriptionName()
    {
        // Act
        var queueName = EventBusAzureServiceBus.GetSubscriptionName<TestDomainEventHandler, TestDomainEvent>();

        // Assert
        queueName.Should().Be("TestModule.Test");
    }

    [Fact]
    public void GetSubscriptionNameTruncatesNameAt50Characters()
    {
        // Act
        var queueName = EventBusAzureServiceBus.GetSubscriptionName<VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEventHandler, VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEvent>();

        // Assert
        queueName.Should().Be("TestModule.VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfT");
    }
}

#pragma warning disable IDE0130
internal class VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEvent : DomainEvent;

internal class VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEventHandler : IDomainEventHandler<VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEvent>
{
    public Task Handle(VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEvent domainEvent)
    {
        // Do nothing
        return Task.CompletedTask;
    }
}
