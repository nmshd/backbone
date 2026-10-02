using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Modules.Tokens.Contracts.DomainEvents;

namespace Backbone.Modules.Synchronization.Module.Features.ExternalEvents.TokenLocked;

public class Handler : IDomainEventHandler<TokenLockedDomainEvent>
{
    private readonly ISynchronizationDbContext _dbContext;

    public Handler(ISynchronizationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(TokenLockedDomainEvent @event)
    {
        if (@event.CreatedBy == null)
            return;

        var payload = new TokenLockedExternalEvent.EventPayload { TokenId = @event.TokenId };
        var externalEvent = new TokenLockedExternalEvent(@event.CreatedBy, payload);

        await _dbContext.CreateExternalEvent(externalEvent);
    }
}
