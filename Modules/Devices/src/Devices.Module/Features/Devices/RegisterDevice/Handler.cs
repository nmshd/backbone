using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Module.Features.Devices.RegisterDevice;

public class Handler : IRequestHandler<RegisterDeviceCommand, RegisterDeviceResponse>
{
    private readonly ChallengeValidator _challengeValidator;
    private readonly ILogger<Handler> _logger;
    private readonly IUserContext _userContext;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(ChallengeValidator challengeValidator, IUserContext userContext, ILogger<Handler> logger, IIdentitiesRepository identitiesRepository)
    {
        _challengeValidator = challengeValidator;
        _userContext = userContext;
        _logger = logger;
        _identitiesRepository = identitiesRepository;
    }

    public async Task<RegisterDeviceResponse> Handle(RegisterDeviceCommand command, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(_userContext.GetAddress(), cancellationToken, track: true) ?? throw new NotFoundException(nameof(Identity));

        if (command.IsBackupDevice && await _identitiesRepository.HasBackupDevice(identity.Address, cancellationToken))
            throw new ApplicationException(ApplicationErrors.Devices.BackupDeviceAlreadyExists());

        await _challengeValidator.Validate(command.SignedChallenge, PublicKey.FromBytes(identity.PublicKey));
        _logger.LogTrace("Successfully validated challenge.");

        var communicationLanguageResult = CommunicationLanguage.Create(command.CommunicationLanguage);

        var newDevice = identity.AddDevice(communicationLanguageResult.Value, _userContext.GetDeviceId(), command.IsBackupDevice);

        await _identitiesRepository.UpdateWithNewDevice(identity, command.DevicePassword);

        _logger.CreatedDevice();

        return new RegisterDeviceResponse(newDevice);
    }
}

internal static partial class DeleteDeviceLogs
{
    [LoggerMessage(
        EventId = 219823,
        EventName = "Devices.RegisterDevice.RegisteredDevice",
        Level = LogLevel.Information,
        Message = "Successfully created device.")]
    public static partial void CreatedDevice(this ILogger logger);
}
