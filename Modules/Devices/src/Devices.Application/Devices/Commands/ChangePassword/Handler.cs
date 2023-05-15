using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Devices.Commands.ChangePassword;

public class Handler : IRequestHandler<ChangePasswordCommand>
{
    private readonly DeviceId _activeDevice;
    private readonly ILogger<Handler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDevicesRepository _devicesRepository;

    public Handler(UserManager<ApplicationUser> userManager, IUserContext userContext, ILogger<Handler> logger, IDevicesRepository devicesRepository)
    {
        _userManager = userManager;
        _logger = logger;
        _activeDevice = userContext.GetDeviceId();
        _devicesRepository = devicesRepository;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var currentDevice = await _devicesRepository.GetDeviceById(_activeDevice, cancellationToken);

        var changePasswordResult = await _userManager.ChangePasswordAsync(currentDevice.User, request.OldPassword, request.NewPassword);

        if (!changePasswordResult.Succeeded)
            if (!changePasswordResult.Succeeded)
                throw new OperationFailedException(ApplicationErrors.Devices.ChangePasswordFailed(changePasswordResult.Errors.First().Description));

        _logger.LogTrace($"Successfully changed password for device with id '{_activeDevice}'.");
    }
}
