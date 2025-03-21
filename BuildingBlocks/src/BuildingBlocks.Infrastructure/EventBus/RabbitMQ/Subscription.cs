﻿using RabbitMQ.Client.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;

public class Subscription
{
    public Subscription(AsyncEventingBasicConsumer consumer, string queueName)
    {
        Consumer = consumer;
        QueueName = queueName;
    }

    public AsyncEventingBasicConsumer Consumer { get; }
    public string QueueName { get; }
}
