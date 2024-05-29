using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Domain;
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
        var currentDevice = await _identitiesRepository.GetDeviceById(_activeDevice, cancellationToken, track: true) ?? throw new NotFoundException(nameof(Device));

        var communicationLanguage = CommunicationLanguage.Create(request.CommunicationLanguage);
        if (communicationLanguage.IsFailure)
            throw new DomainException(communicationLanguage.Error);

        var deviceUpdated = currentDevice.Update(communicationLanguage.Value);
        if (deviceUpdated)
            await _identitiesRepository.Update(currentDevice, cancellationToken);

        _logger.UpdatedDevice(_activeDevice);
    }
}

internal static partial class UpdateDeviceLogs
{
    [LoggerMessage(
        EventId = 274194,
        EventName = "Devices.UpdateActiveDevice.UpdatedDevice",
        Level = LogLevel.Information,
        Message = "Successfully updated device with id '{activeDevice}'.")]
    public static partial void UpdatedDevice(this ILogger logger, DeviceId activeDevice);
}
