using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestIntegrationEvents;
using FluentAssertions;
using Polly;
using Xunit.Sdk;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestIntegrationEventHandlers;

public class TestEvent2IntegrationEventHandler : IIntegrationEventHandler<TestEvent2IntegrationEvent>
{
    public static List<TestEvent2IntegrationEventHandler> Instances { get; } = [];

    public TestEvent2IntegrationEventHandler()
    {
        Instances.Add(this);
    }

    public bool Triggered { get; set; }

    public Task Handle(TestEvent2IntegrationEvent @event)
    {
        Triggered = true;
        return Task.CompletedTask;
    }

    public static void ShouldEventuallyHaveOneTriggeredInstance()
    {
        Policy
            .Handle<XunitException>()
            .WaitAndRetry(retryCount: 5, _ => TimeSpan.FromSeconds(1))
            .Execute(() =>
            {
                Instances.Should().HaveCount(1);
                Instances[0].Triggered.Should().BeTrue();
            });
    }
}
