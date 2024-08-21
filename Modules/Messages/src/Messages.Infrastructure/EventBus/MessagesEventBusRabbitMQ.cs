using Autofac;
using Backbone.BuildingBlocks.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Infrastructure.EventBus.RabbitMQ;
using Backbone.Modules.Messages.Application.Infrastructure.EventBus;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Messages.Infrastructure.EventBus;
public class MessagesEventBusRabbitMq : EventBusRabbitMq, IMessagesEventBus
{
    public MessagesEventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILogger<EventBusRabbitMq> logger, ILifetimeScope autofac,
        IHttpContextAccessor httpContextAccessor, IEventBusSubscriptionsManager subsManager, HandlerRetryBehavior handlerRetryBehavior, string? queueName = null) : base(
        persistentConnection, logger, autofac, httpContextAccessor, subsManager, handlerRetryBehavior, queueName)
    {
    }
}
