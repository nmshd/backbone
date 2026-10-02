using System.Collections;
using System.Collections.Concurrent;
using RabbitMQ.Client.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public class SubscriptionManager : IEnumerable<Subscription>
{
    private readonly ConcurrentQueue<Subscription> _subscriptions = new();

    public void AddSubscription(AsyncEventingBasicConsumer consumer, string queueName)
    {
        _subscriptions.Enqueue(new Subscription(consumer, queueName));
    }

    public IEnumerator<Subscription> GetEnumerator()
    {
        return _subscriptions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
