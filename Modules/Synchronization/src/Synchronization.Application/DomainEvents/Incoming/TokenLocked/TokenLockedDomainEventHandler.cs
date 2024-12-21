using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.TokenLocked;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;

namespace Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.TokenLocked;

public class TokenLockedDomainEventHandler : IDomainEventHandler<TokenLockedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;

    public TokenLockedDomainEventHandler(ISynchronizationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(TokenLockedDomainEvent @event)
    {
        var payload = new TokenLockedExternalEvent.EventPayload { TokenId = @event.TokenId };
        var externalEvent = new TokenLockedExternalEvent(@event.CreatedBy, payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }
}
