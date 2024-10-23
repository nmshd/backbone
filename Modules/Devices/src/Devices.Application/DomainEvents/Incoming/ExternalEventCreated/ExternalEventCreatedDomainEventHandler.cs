using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.ExternalEvents;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.DomainEvents.Incoming.ExternalEventCreated;

public class ExternalEventCreatedDomainEventHandler : IDomainEventHandler<ExternalEventCreatedDomainEvent>
{
    private readonly IPushNotificationSender _pushSenderService;
    private readonly IUserContext _userContext;
    private readonly IIdentitiesRepository _identitiesRepository;

    public ExternalEventCreatedDomainEventHandler(IPushNotificationSender pushSenderService, IUserContext userContext, IIdentitiesRepository identitiesRepository)
    {
        _pushSenderService = pushSenderService;
        _userContext = userContext;
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(ExternalEventCreatedDomainEvent @event)
    {
        if (@event.IsDeliveryBlocked)
            return;

        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), CancellationToken.None) ?? throw new NotFoundException(nameof(Identity));
        
        if(identity.Status != IdentityStatus.ToBeDeleted)
            await _pushSenderService.SendNotification(@event.Owner, new ExternalEventCreatedPushNotification(), CancellationToken.None);
    }
}
