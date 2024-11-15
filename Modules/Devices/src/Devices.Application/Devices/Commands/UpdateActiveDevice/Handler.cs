using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Devices.Commands.UpdateActiveDevice;

public class Handler : IRequestHandler<UpdateActiveDeviceCommand>
{
    private readonly DeviceId _activeDevice;
    private readonly ILogger<Handler> _logger;
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IUserContext userContext, ILogger<Handler> logger, IIdentitiesRepository identitiesRepository)
    {
        _logger = logger;
        _activeDevice = userContext.GetDeviceId();
        _identitiesRepository = identitiesRepository;
    }

    public async Task Handle(UpdateActiveDeviceCommand request, CancellationToken cancellationToken)
    {
        var currentDevice = await _identitiesRepository.GetDeviceById(_activeDevice, cancellationToken, track: true) ?? throw new Exception("Active device could not be found.");

        var communicationLanguage = CommunicationLanguage.Create(request.CommunicationLanguage);
        if (communicationLanguage.IsFailure)
            throw new DomainException(communicationLanguage.Error);

        var deviceUpdated = currentDevice.Update(communicationLanguage.Value);
        if (deviceUpdated)
            await _identitiesRepository.Update(currentDevice, cancellationToken);

        _logger.UpdatedActiveDevice();
    }
}

internal static partial class UpdateActiveDeviceLogs
{
    [LoggerMessage(
        EventId = 274194,
        EventName = "Devices.UpdateActiveDevice.UpdatedActiveDevice",
        Level = LogLevel.Information,
        Message = "Successfully updated the device.")]
    public static partial void UpdatedActiveDevice(this ILogger logger);
}
