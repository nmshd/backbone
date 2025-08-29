using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Tokens;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.TokenLocked;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.DomainEvents.Incoming.TokenLocked;

public class TokenLockedDomainEventHandler : IDomainEventHandler<TokenLockedDomainEvent>
{
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly IIdentitiesRepository _identitiesRepository;

    public TokenLockedDomainEventHandler(IPushNotificationSender pushNotificationSender, IIdentitiesRepository identitiesRepository)
    {
        _pushNotificationSender = pushNotificationSender;
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(TokenLockedDomainEvent @event)
    {
        var identity = await _identitiesRepository.Get(@event.CreatedBy, CancellationToken.None) ?? throw new NotFoundException(nameof(Identity));

        if (identity.Status is not IdentityStatus.ToBeDeleted)
            await _pushNotificationSender.SendNotification(
                new TokenLockedPushNotification(),
                SendPushNotificationFilter.AllDevicesOf(identity.Address),
                CancellationToken.None);
    }
}
