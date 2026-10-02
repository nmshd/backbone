using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Module.Features.Devices.ChangePassword;

public class Handler : IRequestHandler<Command>
{
    private readonly DeviceId _activeDevice;
    private readonly ILogger<Handler> _logger;
    private readonly IDevicePasswordService _passwordService;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IDevicePasswordService passwordService, IUserContext userContext, ILogger<Handler> logger, IIdentitiesRepository identitiesRepository)
    {
        _passwordService = passwordService;
        _logger = logger;
        _activeDevice = userContext.GetDeviceId();
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var currentDevice = await _identitiesRepository.Get(_activeDevice, cancellationToken, track: true) ?? throw new NotFoundException(nameof(Device));

        var error = await _passwordService.ChangePassword(currentDevice.User, request.OldPassword, request.NewPassword);

        if (error != null)
            throw new OperationFailedException(ApplicationErrors.Devices.ChangePasswordFailed(error));

        _logger.ChangedPasswordForDevice();
    }
}

internal static partial class ChangePasswordLogs
{
    [LoggerMessage(
        EventId = 277894,
        EventName = "Devices.ChangePassword.ChangedPasswordForDevice",
        Level = LogLevel.Information,
        Message = "Successfully changed password for the device.")]
    public static partial void ChangedPasswordForDevice(this ILogger logger);
}
