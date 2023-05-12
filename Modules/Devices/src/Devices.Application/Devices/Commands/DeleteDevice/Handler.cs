using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;

public class Handler : IRequestHandler<DeleteDeviceCommand>
{
    private readonly ChallengeValidator _challengeValidator;
    private readonly ILogger<Handler> _logger;
    private readonly IUserContext _userContext;
    private readonly IDevicesRepository _devicesRepository;

    public Handler(IDevicesRepository devicesRepository, IUserContext userContext, ChallengeValidator challengeValidator, ILogger<Handler> logger)
    {
        _devicesRepository = devicesRepository;
        _userContext = userContext;
        _challengeValidator = challengeValidator;
        _logger = logger;
    }

    public async Task Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await _devicesRepository.GetDeviceByIdentityAndId(_userContext.GetAddress(), request.DeviceId, cancellationToken);

        await _challengeValidator.Validate(request.SignedChallenge, PublicKey.FromBytes(device.Identity.PublicKey));

        _logger.LogTrace("Challenge successfully validated.");

        await _devicesRepository.MarkAsDeleted(device.Id, request.DeletionCertificate, _userContext.GetDeviceId(), cancellationToken);

        _logger.LogTrace($"Successfully marked device with id '{request.DeviceId}' as deleted.");
    }
}
