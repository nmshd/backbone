using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
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
        var deviceId = DeviceId.Parse(request.DeviceId);
        var deviceThatIsBeingDeleted = await _identitiesRepository.GetDeviceById(deviceId, cancellationToken, track: true) ?? throw new NotFoundException(nameof(Device));

        deviceThatIsBeingDeleted.MarkAsDeleted(_userContext.GetDeviceId(), _userContext.GetAddress());
        await _identitiesRepository.Update(deviceThatIsBeingDeleted, cancellationToken);

        _logger.MarkedDeviceAsDeleted();
    }
}

internal static partial class DeleteDeviceLogs
{
    [LoggerMessage(
        EventId = 776010,
        EventName = "Devices.MarkDeviceAsDeleted.MarkedDeviceAsDeleted",
        Level = LogLevel.Information,
        Message = "Successfully marked the device as deleted.")]
    public static partial void MarkedDeviceAsDeleted(this ILogger logger);
}
