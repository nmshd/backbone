using System.Globalization;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Logging;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Devices.Commands.UpdateDevice;

public class Handler : IRequestHandler<UpdateDeviceCommand>
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

    public async Task Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        var currentDevice = await _identitiesRepository.GetDeviceById(_activeDevice, cancellationToken, true) ?? throw new NotFoundException(nameof(Device));

        var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
        var culture = cultureInfos.FirstOrDefault(c => c.TwoLetterISOLanguageName == request.CommunicationLanguage) ??
                      throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPropertyValue(nameof(Device.CommunicationLanguage)));

        var deviceUpdated = currentDevice.Update(culture.TwoLetterISOLanguageName);
        if (deviceUpdated)
            await _identitiesRepository.Update(currentDevice, cancellationToken);

        _logger.UpdatedDevice(_activeDevice);
    }
}

internal static partial class UpdateDeviceLogs
{
    [LoggerMessage(
        EventId = 274194,
        EventName = "Devices.UpdateDevice.UpdatedDevice",
        Level = LogLevel.Information,
        Message = "Successfully updated device with id '{activeDevice}'.")]
    public static partial void UpdatedDevice(this ILogger logger, DeviceId activeDevice);
}
