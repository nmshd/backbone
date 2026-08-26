using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using OpenTelemetry;

namespace Backbone.BuildingBlocks.API.Diagnostics;

public sealed class UserContextProcessor : BaseProcessor<Activity>
{
    private readonly IUserContext _userContext;

    public UserContextProcessor(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public override void OnEnd(Activity data)
    {
        data.SetTag("clientId", _userContext.GetClientIdOrNull() ?? Baggage.Current.GetBaggage("clientId"));
        data.SetTag("deviceId", _userContext.GetDeviceIdOrNull() ?? Baggage.Current.GetBaggage("deviceId"));
        data.SetTag("identityAddress", _userContext.GetAddressOrNull()?.Value ?? Baggage.Current.GetBaggage("identityAddress"));
        data.SetTag("username", _userContext.GetUsernameOrNull() ?? Baggage.Current.GetBaggage("username"));
    }
}
