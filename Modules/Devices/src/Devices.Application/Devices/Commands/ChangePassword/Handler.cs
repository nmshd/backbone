using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Devices.Commands.ChangePassword;

public class Handler : IRequestHandler<ChangePasswordCommand>
{
    private readonly DeviceId _activeDevice;
    private readonly ILogger<Handler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(UserManager<ApplicationUser> userManager, IUserContext userContext, ILogger<Handler> logger, IIdentitiesRepository identitiesRepository)
    {
        _userManager = userManager;
        _logger = logger;
        _activeDevice = userContext.GetDeviceId();
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var currentDevice = await _identitiesRepository.Get(_activeDevice, cancellationToken, track: true) ?? throw new NotFoundException(nameof(Device));

        var changePasswordResult = await _userManager.ChangePasswordAsync(currentDevice.User, request.OldPassword, request.NewPassword);

        if (!changePasswordResult.Succeeded)
            throw new OperationFailedException(ApplicationErrors.Devices.ChangePasswordFailed(changePasswordResult.Errors.First().Description));

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
