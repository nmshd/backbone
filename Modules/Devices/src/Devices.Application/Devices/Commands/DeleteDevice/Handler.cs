using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

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
        var device = await _identitiesRepository.GetDeviceById(request.DeviceId, cancellationToken, track: true);

        if (device.Identity.Address != _userContext.GetAddress())
        {
            throw new NotFoundException(nameof(device));
        }

        _logger.LogTrace("Challenge successfully validated.");

        device.MarkAsDeleted(request.DeviceId);

        await _identitiesRepository.Update(device, cancellationToken);

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
