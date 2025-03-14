using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;
using Backbone.Modules.TestModule.Application.DomainEvents;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.AzureServiceBus
{
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
        public void GetSubscriptionName_truncates_name_at_50_characters()
        {
            // Act
            var queueName = EventBusAzureServiceBus.GetSubscriptionName<VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEventHandler, VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEvent>();

            // Assert
            queueName.Should().Be("TestModule.VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfT");
        }
    }
}

#pragma warning disable IDE0130
namespace Backbone.Modules.TestModule.Application.DomainEvents
#pragma warning restore IDE0130
{
    internal class VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEvent : DomainEvent;

    internal class VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEventHandler : IDomainEventHandler<VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEvent>
    {
        public Task Handle(VeeeeeeeeeeeeeeeeeeeeeeeeeryLongNameOfTestDomainEvent domainEvent)
        {
            // Do nothing
            return Task.CompletedTask;
        }
    }
}
