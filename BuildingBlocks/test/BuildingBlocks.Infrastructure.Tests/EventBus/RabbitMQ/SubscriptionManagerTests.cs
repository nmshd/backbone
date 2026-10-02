using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using FakeItEasy;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Backbone.BuildingBlocks.Infrastructure.Tests.EventBus.RabbitMQ;

public class SubscriptionManagerTests : AbstractTestsBase
{
    [Fact]
    public void Preserves_all_subscriptions_registered_concurrently()
    {
        // Arrange
        const int numberOfSubscriptions = 10000;
        var manager = new SubscriptionManager();
        var consumer = new AsyncEventingBasicConsumer(A.Fake<IChannel>());

        // Act
        Parallel.For(0, numberOfSubscriptions, index => manager.AddSubscription(consumer, index.ToString()));

        // Assert
        manager.Select(subscription => int.Parse(subscription.QueueName)).Order()
            .ShouldBe(Enumerable.Range(0, numberOfSubscriptions));
    }
}
