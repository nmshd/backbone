using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Devices.Commands.DeleteDevice;

public class Handler : IRequestHandler<DeleteDeviceCommand>
{
    private readonly ChallengeValidator _challengeValidator;
    private readonly IDevicesDbContext _dbContext;
    private readonly ILogger<Handler> _logger;
    private readonly IUserContext _userContext;

    public Handler(IDevicesDbContext dbContext, IUserContext userContext, ChallengeValidator challengeValidator, ILogger<Handler> logger)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _challengeValidator = challengeValidator;
        _logger = logger;
    }

    public async Task Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await _dbContext.Set<Device>()
            .OfIdentity(_userContext.GetAddress())
            .NotDeleted()
            .Include(d => d.Identity)
            .FirstWithId(request.DeviceId, cancellationToken);

        await _challengeValidator.Validate(request.SignedChallenge, PublicKey.FromBytes(device.Identity.PublicKey));

        _logger.LogTrace("Challenge successfully validated.");

        device.MarkAsDeleted(request.DeletionCertificate, _userContext.GetDeviceId());

        _dbContext.Set<Device>().Update(device);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogTrace($"Successfully marked device with id '{request.DeviceId}' as deleted.");
    }
}
