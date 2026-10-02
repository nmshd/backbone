using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Module.Features.Devices.DeleteDevice;

public class Handler : IRequestHandler<Command>
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

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var deviceId = DeviceId.Parse(request.DeviceId);
        var deviceThatIsBeingDeleted = await _identitiesRepository.Get(deviceId, cancellationToken, track: true) ?? throw new NotFoundException(nameof(Device));

        deviceThatIsBeingDeleted.EnsureCanBeDeleted(_userContext.GetAddress());

        await _identitiesRepository.DeleteDevice(deviceThatIsBeingDeleted, cancellationToken);

        _logger.DeviceDeleted();
    }
}

internal static partial class DeleteDeviceLogs
{
    [LoggerMessage(
        EventId = 776010,
        EventName = "Devices.DeleteDevice.DeviceDeleted",
        Level = LogLevel.Information,
        Message = "The device was deleted.")]
    public static partial void DeviceDeleted(this ILogger logger);
}
