using Autofac;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Backbone.Modules.Messages.Application.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Messages.Infrastructure.EventBus;
public class MessagesEventBusRabbitMq : EventBusRabbitMq, IMessagesEventBus
{
    public MessagesEventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILogger<EventBusRabbitMq> logger, ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, HandlerRetryBehavior handlerRetryBehavior, string? queueName = null) : base(persistentConnection, logger, autofac, subsManager, handlerRetryBehavior, queueName)
    {
    }
}
