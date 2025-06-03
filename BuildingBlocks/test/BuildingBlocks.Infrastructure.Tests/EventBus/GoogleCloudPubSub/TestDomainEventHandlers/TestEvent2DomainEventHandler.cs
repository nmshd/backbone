using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestDomainEvents;
using Backbone.UnitTestTools.Shouldly.Extensions;
using Polly;
using Xunit.Sdk;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestDomainEventHandlers;

public class TestEvent2DomainEventHandler : IDomainEventHandler<TestEvent2DomainEvent>
{
    public static List<TestEvent2DomainEventHandler> Instances { get; } = [];

    public TestEvent2DomainEventHandler()
    {
        Instances.Add(this);
    }

    public bool Triggered { get; set; }

    public Task Handle(TestEvent2DomainEvent event2DomainEvent)
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
                Instances.ShouldHaveCount(1);
                Instances[0].Triggered.ShouldBeTrue();
            });
    }
}
