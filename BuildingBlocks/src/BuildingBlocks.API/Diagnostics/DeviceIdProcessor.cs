using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using OpenTelemetry;

namespace Backbone.BuildingBlocks.API.Diagnostics;

public sealed class DeviceIdProcessor : BaseProcessor<Activity>
{
    private readonly IUserContext _userContext;

    public DeviceIdProcessor(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public override void OnEnd(Activity data)
    {
        data.SetTag("deviceId", _userContext.GetDeviceIdOrNull());
    }
}
