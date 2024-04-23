// using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
// using Microsoft.Extensions.Logging;
// using Synchronization.Application.DomainEvents.Outgoing;
// using Synchronization.Domain.Entities.Sync;
//
// namespace Synchronization.Application.DomainEvents.Incoming.MessageDelivered;
//
// public class MessageDeliveredDomainEventHandler : IDomainEventHandler<MessageDeliveredDomainEvent>
// {
//     private readonly ISynchronizationDbContext _dbContext;
//     private readonly IEventBus _eventBus;
//     private readonly ILogger<MessageDeliveredDomainEventHandler> _logger;
//
//     public MessageDeliveredDomainEventHandler(ISynchronizationDbContext dbContext, IEventBus eventBus, ILogger<MessageDeliveredDomainEventHandler> logger)
//     {
//         _dbContext = dbContext;
//         _eventBus = eventBus;
//         _logger = logger;
//     }
//
//     public async Task Handle(MessageDeliveredDomainEvent domainEvent)
//     {
//         await CreateExternalEvent(domainEvent);
//     }
//
//     private async Task CreateExternalEvent(MessageDeliveredDomainEvent domainEvent)
//     {
//         var payload = new {Id = domainEvent.MessageId};
//         try
//         {
//             var externalEvent = await _dbContext.CreateExternalEvent(domainEvent.Sender, ExternalEventType.MessageDelivered, payload);
//             _eventBus.Publish(new ExternalEventCreatedDomainEvent(externalEvent));
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while processing an domain event.");
//             throw;
//         }
//     }
// }



