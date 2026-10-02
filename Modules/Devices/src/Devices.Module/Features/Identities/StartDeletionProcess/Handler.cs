using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.DeletionProcess;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.StartDeletionProcess;

public class Handler : IRequestHandler<Command, Response>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;
    private readonly IPushNotificationSender _notificationSender;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext, IPushNotificationSender notificationSender)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
        _notificationSender = notificationSender;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(_userContext.GetAddress(), cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        var deletionProcess = identity.StartDeletionProcess(_userContext.GetDeviceId(), request.LengthOfGracePeriodInDays);

        try
        {
            await _identitiesRepository.Update(identity, cancellationToken);
        }
        catch (OnlyOneActiveDeletionProcessAllowedException)
        {
            throw new DomainException(DomainErrors.OnlyOneActiveDeletionProcessAllowed());
        }

        await _notificationSender.SendNotification(
            new DeletionProcessStartedPushNotification(),
            SendPushNotificationFilter.AllDevicesOf(identity.Address),
            cancellationToken
        );

        return new Response(deletionProcess);
    }
}
