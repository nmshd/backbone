﻿using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestIntegrationEvents;
using FluentAssertions;
using FluentAssertions.Extensions;
using Polly;
using Xunit.Sdk;

namespace Enmeshed.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestIntegrationEventHandlers;

public class TestEvent1IntegrationEventHandler2 : IIntegrationEventHandler<TestEvent1IntegrationEvent>
{
    public static List<TestEvent1IntegrationEventHandler2> Instances { get; } = new();

    public TestEvent1IntegrationEventHandler2()
    {
        Instances.Add(this);
    }

    public bool Triggered { get; set; }

    public Task Handle(TestEvent1IntegrationEvent @event)
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
