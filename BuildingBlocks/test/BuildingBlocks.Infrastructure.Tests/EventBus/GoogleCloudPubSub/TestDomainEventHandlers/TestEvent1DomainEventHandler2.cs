using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestDomainEvents;
using FluentAssertions.Extensions;
using Polly;
using Xunit.Sdk;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestDomainEventHandlers;

public class TestEvent1DomainEventHandler2 : IDomainEventHandler<TestEvent1DomainEvent>
{
    public static List<TestEvent1DomainEventHandler2> Instances { get; } = [];

    public TestEvent1DomainEventHandler2()
    {
        Instances.Add(this);
    }

    public bool Triggered { get; set; }

    public Task Handle(TestEvent1DomainEvent event1DomainEvent)
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

    public static void ShouldNotHaveAnyTriggeredInstance()
    {
        Task.Delay(5.Seconds()).Wait();
        Instances.Should().HaveCount(0);
    }
}
