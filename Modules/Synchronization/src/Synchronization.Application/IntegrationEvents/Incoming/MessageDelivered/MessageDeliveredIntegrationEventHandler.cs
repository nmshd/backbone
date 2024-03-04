// using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
// using Microsoft.Extensions.Logging;
// using Synchronization.Application.IntegrationEvents.Outgoing;
// using Synchronization.Domain.Entities.Sync;
//
// namespace Synchronization.Application.IntegrationEvents.Incoming.MessageDelivered;
//
// public class MessageDeliveredIntegrationEventHandler : IIntegrationEventHandler<MessageDeliveredIntegrationEvent>
// {
//     private readonly ISynchronizationDbContext _dbContext;
//     private readonly IEventBus _eventBus;
//     private readonly ILogger<MessageDeliveredIntegrationEventHandler> _logger;
//
//     public MessageDeliveredIntegrationEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<MessageDeliveredIntegrationEventHandler> logger)
//     {
//         _dbContext = dbContext;
//         _eventBus = eventBus;
//         _logger = logger;
//     }
//
//     public async Task Handle(MessageDeliveredIntegrationEvent integrationEvent)
//     {
//         await CreateExternalEvent(integrationEvent);
//     }
//
//     private async Task CreateExternalEvent(MessageDeliveredIntegrationEvent integrationEvent)
//     {
//         var payload = new {Id = integrationEvent.MessageId};
//         try
//         {
//             var externalEvent = await _dbContext.CreateExternalEvent(integrationEvent.Sender, ExternalEventType.MessageDelivered, payload);
//             _eventBus.Publish(new ExternalEventCreatedIntegrationEvent(externalEvent));
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while processing an integration event.");
//             throw;
//         }
//     }
// }
