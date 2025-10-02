using System.Collections;
using RabbitMQ.Client.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public class SubscriptionManager : IEnumerable<Subscription>
{
    private readonly IList<Subscription> _subscriptions = [];

    public void AddSubscription(AsyncEventingBasicConsumer consumer, string queueName)
    {
        _subscriptions.Add(new Subscription(consumer, queueName));
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
