using Autofac;
using Backbone.Modules.Messages.Application.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Messages.Infrastructure.EventBus;
public class MessagesEventBusRabbitMq : EventBusRabbitMq, IMessagesEventBus
{
    public MessagesEventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILogger<EventBusRabbitMq> logger, ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string queueName = null, int retryCount = 5) : base(persistentConnection, logger, autofac, subsManager, queueName, retryCount)
    {
    }
}
