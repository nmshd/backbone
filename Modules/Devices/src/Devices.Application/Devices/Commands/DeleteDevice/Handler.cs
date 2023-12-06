using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;

public class Handler : IRequestHandler<DeleteDeviceCommand>
{
    private readonly ILogger<Handler> _logger;
    private readonly IUserContext _userContext;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext, ILogger<Handler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var deviceThatIsDeletingId = _userContext.GetDeviceId();
        var deviceThatIsDeleting = await _identitiesRepository.GetDeviceById(deviceThatIsDeletingId, cancellationToken, track: true);

        if (deviceThatIsDeleting.Identity.Address != _userContext.GetAddress())
            throw new NotFoundException(nameof(deviceThatIsDeleting));

        var deviceThatIsBeingDeleted = await _identitiesRepository.GetDeviceById(request.DeviceId, cancellationToken, track: true);

        if (deviceThatIsBeingDeleted.Identity.Address != _userContext.GetAddress())
            throw new NotFoundException(nameof(deviceThatIsBeingDeleted));

        deviceThatIsBeingDeleted.MarkAsDeleted(deviceThatIsDeletingId);
        await _identitiesRepository.Update(deviceThatIsBeingDeleted, cancellationToken);

        _logger.MarkedDeviceAsDeleted(request.DeviceId);
    }
}

internal static partial class DeleteDeviceLogs
{
    [LoggerMessage(
        EventId = 776010,
        EventName = "Devices.MarkDeviceAsDeleted.MarkedDeviceAsDeleted",
        Level = LogLevel.Information,
        Message = "Successfully marked device with id '{deviceId}' as deleted.")]
    public static partial void MarkedDeviceAsDeleted(this ILogger logger, string deviceId);
}
