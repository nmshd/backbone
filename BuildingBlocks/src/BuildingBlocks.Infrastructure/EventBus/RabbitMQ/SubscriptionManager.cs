using RabbitMQ.Client.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public class SubscriptionManager
{
    private readonly IList<Subscription> _subscriptions = [];

    public void AddSubscription(AsyncEventingBasicConsumer consumer, string queueName)
    {
        _subscriptions.Add(new Subscription(consumer, queueName));
    }

    public IReadOnlyList<Subscription> Subscriptions => _subscriptions.AsReadOnly();
}
