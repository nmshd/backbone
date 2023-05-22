using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;

public class Handler : IRequestHandler<DeleteDeviceCommand>
{
    private readonly ChallengeValidator _challengeValidator;
    private readonly ILogger<Handler> _logger;
    private readonly IUserContext _userContext;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext, ChallengeValidator challengeValidator, ILogger<Handler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
        _challengeValidator = challengeValidator;
        _logger = logger;
    }

    public async Task Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await _identitiesRepository.GetDeviceById(request.DeviceId, cancellationToken, track: true);

        if(device.Identity.Address != _userContext.GetAddress()) {
            throw new NotFoundException(nameof(device));
        }
        
        await _challengeValidator.Validate(request.SignedChallenge, PublicKey.FromBytes(device.Identity.PublicKey));

        _logger.LogTrace("Challenge successfully validated.");

        device.MarkAsDeleted(request.DeletionCertificate, _userContext.GetDeviceId());

        await _identitiesRepository.Update(device, cancellationToken);

        _logger.LogTrace($"Successfully marked device with id '{request.DeviceId}' as deleted.");
    }
}
