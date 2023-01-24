using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Devices.API.ApplicationInsights.TelemetryInitializers;

public class UserInformationTelemetryInitializer : ITelemetryInitializer
{
    private readonly IUserContext _userContext;

    public UserInformationTelemetryInitializer(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is not RequestTelemetry requestTelemetry)
            return;

        var address = _userContext.GetAddressOrNull() ?? "";
        var deviceId = _userContext.GetDeviceIdOrNull() ?? "";

        requestTelemetry.Properties.Add("Address", address);
        requestTelemetry.Properties.Add("DeviceId", deviceId);
    }
}
