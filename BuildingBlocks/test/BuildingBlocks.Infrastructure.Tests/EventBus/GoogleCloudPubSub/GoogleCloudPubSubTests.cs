﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Divergic.Logging.Xunit;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Enmeshed.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestIntegrationEventHandlers;
using Enmeshed.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestIntegrationEvents;
using Enmeshed.Tooling.Extensions;
using FluentAssertions;
using FluentAssertions.Extensions;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Enmeshed.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub;

public class GoogleCloudPubSubTests : IDisposable
{
    private readonly EventBusFactory _factory;

    public GoogleCloudPubSubTests(ITestOutputHelper output)
    {
        _factory = new EventBusFactory(output);
    }

    public void Dispose()
    {
        _factory.Dispose();

        TestEvent1IntegrationEventHandler1.Instances.Clear();
        TestEvent1IntegrationEventHandler2.Instances.Clear();
        TestEvent2IntegrationEventHandler.Instances.Clear();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public void One_subscriber_for_one_event()
    {
        Task.Delay(30.Seconds()).GetAwaiter().GetResult();

        var subscriber = _factory.CreateEventBus();
        var publisher = _factory.CreateEventBus();

        subscriber.Subscribe<TestEvent1IntegrationEvent, TestEvent1IntegrationEventHandler1>();

        publisher.Publish(new TestEvent1IntegrationEvent());

        TestEvent1IntegrationEventHandler1.ShouldEventuallyHaveOneTriggeredInstance();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public void Subscribe_to_the_same_event_twice_with_the_same_subscriber()
    {
        Task.Delay(30.Seconds()).GetAwaiter().GetResult();

        var subscriber = _factory.CreateEventBus();
        var publisher = _factory.CreateEventBus();

        subscriber.Subscribe<TestEvent1IntegrationEvent, TestEvent1IntegrationEventHandler1>();
        subscriber.Subscribe<TestEvent1IntegrationEvent, TestEvent1IntegrationEventHandler2>();

        publisher.Publish(new TestEvent1IntegrationEvent());

        TestEvent1IntegrationEventHandler1.ShouldEventuallyHaveOneTriggeredInstance();
        TestEvent1IntegrationEventHandler2.ShouldEventuallyHaveOneTriggeredInstance();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public void Two_subscribers_for_the_same_event_both_receive_the_event()
    {
        Task.Delay(30.Seconds()).GetAwaiter().GetResult();

        var subscriber1 = _factory.CreateEventBus("subscription1");
        var subscriber2 = _factory.CreateEventBus("subscription2");
        var publisher = _factory.CreateEventBus();

        subscriber1.Subscribe<TestEvent1IntegrationEvent, TestEvent1IntegrationEventHandler1>();
        subscriber2.Subscribe<TestEvent1IntegrationEvent, TestEvent1IntegrationEventHandler2>();

        publisher.Publish(new TestEvent1IntegrationEvent());

        TestEvent1IntegrationEventHandler1.ShouldEventuallyHaveOneTriggeredInstance();
        TestEvent1IntegrationEventHandler2.ShouldEventuallyHaveOneTriggeredInstance();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task Only_one_instance_of_a_subscriber_receives_the_event()
    {
        await Task.Delay(30.Seconds());

        var subscriber1A = _factory.CreateEventBus("subscription1");
        var subscriber1B = _factory.CreateEventBus("subscription1");
        var publisher = _factory.CreateEventBus();

        subscriber1A.Subscribe<TestEvent1IntegrationEvent, TestEvent1IntegrationEventHandler1>();
        subscriber1B.Subscribe<TestEvent1IntegrationEvent, TestEvent1IntegrationEventHandler2>();

        publisher.Publish(new TestEvent1IntegrationEvent());

        await Task.Delay(5.Seconds()); // wait some time to make sure all subscribers were notified

        var numberOfTriggeredInstancesOfHandler1 =
            TestEvent1IntegrationEventHandler1.Instances.Count(i => i.Triggered);
        var numberOfTriggeredInstancesOfHandler2 =
            TestEvent1IntegrationEventHandler2.Instances.Count(i => i.Triggered);

        var totalNumberOfTriggeredInstances =
            numberOfTriggeredInstancesOfHandler1 + numberOfTriggeredInstancesOfHandler2;

        totalNumberOfTriggeredInstances.Should().Be(1);
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public void The_correct_event_handler_is_called_when_multiple_subscriptions_exist()
    {
        Task.Delay(30.Seconds()).GetAwaiter().GetResult();

        var subscriber1 = _factory.CreateEventBus();
        var publisher = _factory.CreateEventBus();

        subscriber1.Subscribe<TestEvent1IntegrationEvent, TestEvent1IntegrationEventHandler1>();
        subscriber1.Subscribe<TestEvent2IntegrationEvent, TestEvent2IntegrationEventHandler>();

        publisher.Publish(new TestEvent1IntegrationEvent());

        TestEvent1IntegrationEventHandler1.ShouldEventuallyHaveOneTriggeredInstance();
        TestEvent1IntegrationEventHandler2.ShouldNotHaveAnyTriggeredInstance();
    }
}

public class EventBusFactory : IDisposable
{
    public record Instance(AutofacServiceProvider AutofacServiceProviders, EventBusGoogleCloudPubSub EventBusClient,
        DefaultGoogleCloudPubSubPersisterConnection PersisterConnection);

    public const string PROJECT_ID = "nbp-nmshd-bkb";
    public const string TOPIC_NAME = "test-topic";
    public const string SUBSCRIPTION_NAME_PREFIX = "subscription1";

    private readonly ICacheLogger<EventBusGoogleCloudPubSub> _logger;

    public const string CONNECTION_INFO = "";

    private readonly List<Instance> _instances = new();

    public EventBusFactory(ITestOutputHelper output)
    {
        _logger = output.BuildLoggerFor<EventBusGoogleCloudPubSub>();
    }

    public EventBusGoogleCloudPubSub CreateEventBus(string subscriptionNamePrefix = SUBSCRIPTION_NAME_PREFIX)
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<TestEvent1IntegrationEventHandler1>();
        builder.RegisterType<TestEvent1IntegrationEventHandler2>();

        var autofacServiceProvider = new AutofacServiceProvider(builder.Build());
        var lifeTimeScope = autofacServiceProvider.GetRequiredService<ILifetimeScope>();
        var eventBusSubscriptionsManager = new InMemoryEventBusSubscriptionsManager();
        var persisterConnection = new DefaultGoogleCloudPubSubPersisterConnection(PROJECT_ID, TOPIC_NAME,
            subscriptionNamePrefix, CONNECTION_INFO);
        var eventBusClient = new EventBusGoogleCloudPubSub(persisterConnection, _logger,
            eventBusSubscriptionsManager, lifeTimeScope);

        var instance = new Instance(autofacServiceProvider, eventBusClient, persisterConnection);
        _instances.Add(instance);

        return eventBusClient;
    }

    public void Dispose()
    {
        _logger.Dispose();

        foreach (var instance in _instances)
        {
            instance.AutofacServiceProviders.Dispose();
            instance.EventBusClient.Dispose();
            instance.PersisterConnection.Dispose();
        }

        CleanupTestSubscriptions();
    }

    public void CleanupTestSubscriptions()
    {
        var gcpCredentials = CONNECTION_INFO.IsEmpty() ? null : GoogleCredential.FromJson(CONNECTION_INFO);

        var subscriberServiceApiClient = new SubscriberServiceApiClientBuilder
        {
            GoogleCredential = gcpCredentials,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction
        }.Build();

        CleanupSubscription(
            subscriberServiceApiClient,
            SubscriptionName.FromProjectSubscription(PROJECT_ID, "subscription1-TestEvent1")
            );

        CleanupSubscription(
            subscriberServiceApiClient,
            SubscriptionName.FromProjectSubscription(PROJECT_ID, "subscription1-TestEvent2")
            );

        CleanupSubscription(
            subscriberServiceApiClient,
            SubscriptionName.FromProjectSubscription(PROJECT_ID, "subscription2-TestEvent1")
            );

        CleanupSubscription(
            subscriberServiceApiClient,
            SubscriptionName.FromProjectSubscription(PROJECT_ID, "subscription2-TestEvent2")
            );
    }

    public void CleanupSubscription(SubscriberServiceApiClient subscriberServiceApiClient, SubscriptionName subscriptionName)
    {
        try
        {
            subscriberServiceApiClient.GetSubscription(subscriptionName);
            subscriberServiceApiClient.DeleteSubscription(subscriptionName);
        }
        catch (RpcException ex)
        {
            if (ex.Status.StatusCode != StatusCode.NotFound) throw;
        }
    }
}
