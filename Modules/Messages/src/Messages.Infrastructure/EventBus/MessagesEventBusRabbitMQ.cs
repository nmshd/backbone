﻿using Autofac;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Messages.Application.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Messages.Infrastructure.EventBus;
public class MessagesEventBusRabbitMQ : EventBusRabbitMQ, IMessagesEventBus
{
    public MessagesEventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger, ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string queueName = null, int retryCount = 5) : base(persistentConnection, logger, autofac, subsManager, queueName, retryCount)
    {
    }
}
