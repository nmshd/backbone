using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling.Extensions;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;

public class Handler : IRequestHandler<ApproveDeletionProcessCommand, ApproveDeletionProcessResponse>
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

    public async Task<ApproveDeletionProcessResponse> Handle(ApproveDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(_userContext.GetAddress(), cancellationToken, track: true) ?? throw new NotFoundException(nameof(Identity));

        var identityDeletionProcessIdResult = IdentityDeletionProcessId.Create(request.DeletionProcessId);

        if (identityDeletionProcessIdResult.IsFailure)
            throw new DomainException(identityDeletionProcessIdResult.Error);

        var identityDeletionProcessId = identityDeletionProcessIdResult.Value;

        var deletionProcess = identity.ApproveDeletionProcess(identityDeletionProcessId, _userContext.GetDeviceId());

        await _identitiesRepository.Update(identity, cancellationToken);

        if (deletionProcess.GracePeriodEndsAt == null)
            throw new Exception($"Expected '{nameof(deletionProcess.GracePeriodEndsAt)}' to be set but found 'null' instead.");

        var daysUntilDeletion = deletionProcess.GracePeriodEndsAt.Value.DaysUntilDate();

        await _notificationSender.SendNotification(
            new DeletionProcessApprovedPushNotification(daysUntilDeletion),
            SendPushNotificationFilter.AllDevicesOf(identity.Address),
            cancellationToken
        );

        return new ApproveDeletionProcessResponse(deletionProcess);
    }
}
