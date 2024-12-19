using Autofac;
using Autofac.Extensions.DependencyInjection;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.GoogleCloudPubSub;
using Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestDomainEventHandlers;
using Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub.TestDomainEvents;
using Backbone.Tooling.Extensions;
using FakeItEasy;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.GoogleCloudPubSub;

public class GoogleCloudPubSubTests : AbstractTestsBase, IAsyncDisposable
{
    private readonly EventBusFactory _factory;

    public GoogleCloudPubSubTests()
    {
        _factory = new EventBusFactory();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task One_subscriber_for_one_event()
    {
        await Task.Delay(30.Seconds());

        var subscriber = _factory.CreateEventBus();
        var publisher = _factory.CreateEventBus();

        await subscriber.Subscribe<TestEvent1DomainEvent, TestEvent1DomainEventHandler1>();

        await publisher.Publish(new TestEvent1DomainEvent());

        TestEvent1DomainEventHandler1.ShouldEventuallyHaveOneTriggeredInstance();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task Subscribe_to_the_same_event_twice_with_the_same_subscriber()
    {
        await Task.Delay(30.Seconds());

        var subscriber = _factory.CreateEventBus();
        var publisher = _factory.CreateEventBus();

        await subscriber.Subscribe<TestEvent1DomainEvent, TestEvent1DomainEventHandler1>();
        await subscriber.Subscribe<TestEvent1DomainEvent, TestEvent1DomainEventHandler2>();

        await publisher.Publish(new TestEvent1DomainEvent());

        TestEvent1DomainEventHandler1.ShouldEventuallyHaveOneTriggeredInstance();
        TestEvent1DomainEventHandler2.ShouldEventuallyHaveOneTriggeredInstance();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task Two_subscribers_for_the_same_event_both_receive_the_event()
    {
        await Task.Delay(30.Seconds());

        var subscriber1 = _factory.CreateEventBus("subscription1");
        var subscriber2 = _factory.CreateEventBus("subscription2");
        var publisher = _factory.CreateEventBus();

        await subscriber1.Subscribe<TestEvent1DomainEvent, TestEvent1DomainEventHandler1>();
        await subscriber2.Subscribe<TestEvent1DomainEvent, TestEvent1DomainEventHandler2>();

        await publisher.Publish(new TestEvent1DomainEvent());

        TestEvent1DomainEventHandler1.ShouldEventuallyHaveOneTriggeredInstance();
        TestEvent1DomainEventHandler2.ShouldEventuallyHaveOneTriggeredInstance();
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task Only_one_instance_of_a_subscriber_receives_the_event()
    {
        await Task.Delay(30.Seconds());

        var subscriber1A = _factory.CreateEventBus("subscription1");
        var subscriber1B = _factory.CreateEventBus("subscription1");
        var publisher = _factory.CreateEventBus();

        await subscriber1A.Subscribe<TestEvent1DomainEvent, TestEvent1DomainEventHandler1>();
        await subscriber1B.Subscribe<TestEvent1DomainEvent, TestEvent1DomainEventHandler2>();

        await publisher.Publish(new TestEvent1DomainEvent());

        await Task.Delay(5.Seconds()); // wait some time to make sure all subscribers were notified

        var numberOfTriggeredInstancesOfHandler1 =
            TestEvent1DomainEventHandler1.Instances.Count(i => i.Triggered);
        var numberOfTriggeredInstancesOfHandler2 =
            TestEvent1DomainEventHandler2.Instances.Count(i => i.Triggered);

        var totalNumberOfTriggeredInstances =
            numberOfTriggeredInstancesOfHandler1 + numberOfTriggeredInstancesOfHandler2;

        totalNumberOfTriggeredInstances.Should().Be(1);
    }

    [Fact(Skip = "No valid emulator for GCP")]
    public async Task The_correct_event_handler_is_called_when_multiple_subscriptions_exist()
    {
        await Task.Delay(30.Seconds());

        var subscriber1 = _factory.CreateEventBus();
        var publisher = _factory.CreateEventBus();

        await subscriber1.Subscribe<TestEvent1DomainEvent, TestEvent1DomainEventHandler1>();
        await subscriber1.Subscribe<TestEvent2DomainEvent, TestEvent2DomainEventHandler>();

        await publisher.Publish(new TestEvent1DomainEvent());

        TestEvent1DomainEventHandler1.ShouldEventuallyHaveOneTriggeredInstance();
        TestEvent1DomainEventHandler2.ShouldNotHaveAnyTriggeredInstance();
    }

    public override void Dispose()
    {
        Task.Run(async () => await DisposeAsync()).GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();

        TestEvent1DomainEventHandler1.Instances.Clear();
        TestEvent1DomainEventHandler2.Instances.Clear();
        TestEvent2DomainEventHandler.Instances.Clear();
    }

    private class EventBusFactory : IDisposable, IAsyncDisposable
    {
        private record Instance(
            AutofacServiceProvider AutofacServiceProviders,
            EventBusGoogleCloudPubSub EventBusClient);

        private const string PROJECT_ID = "nbp-nmshd-bkb";
        private const string TOPIC_NAME = "test-topic";
        private const string SUBSCRIPTION_NAME_PREFIX = "subscription1";

        private const string CONNECTION_INFO = "";

        private readonly List<Instance> _instances = [];

        public EventBusGoogleCloudPubSub CreateEventBus(string subscriptionNamePrefix = SUBSCRIPTION_NAME_PREFIX)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TestEvent1DomainEventHandler1>();
            builder.RegisterType<TestEvent1DomainEventHandler2>();

            var logger = A.Fake<ILogger<EventBusGoogleCloudPubSub>>();

            var autofacServiceProvider = new AutofacServiceProvider(builder.Build());
            var lifeTimeScope = autofacServiceProvider.GetRequiredService<ILifetimeScope>();
            var eventBusSubscriptionsManager = new InMemoryEventBusSubscriptionsManager();
            var persisterConnection = new DefaultGoogleCloudPubSubPersisterConnection(A.Dummy<ILogger<DefaultGoogleCloudPubSubPersisterConnection>>(), PROJECT_ID, TOPIC_NAME,
                subscriptionNamePrefix, CONNECTION_INFO);
            var eventBusClient = new EventBusGoogleCloudPubSub(
                persisterConnection,
                logger,
                eventBusSubscriptionsManager,
                lifeTimeScope,
                new HandlerRetryBehavior { NumberOfRetries = 5, MinimumBackoff = 2, MaximumBackoff = 120 });

            var instance = new Instance(autofacServiceProvider, eventBusClient);
            _instances.Add(instance);

            return eventBusClient;
        }

        public void Dispose()
        {
            Task.Run(async () => await DisposeAsync()).GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var instance in _instances)
            {
                instance.AutofacServiceProviders.Dispose();
                await instance.EventBusClient.DisposeAsync();
            }

            CleanupTestSubscriptions();
        }

        private void CleanupTestSubscriptions()
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

        private static void CleanupSubscription(SubscriberServiceApiClient subscriberServiceApiClient, SubscriptionName subscriptionName)
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
}
