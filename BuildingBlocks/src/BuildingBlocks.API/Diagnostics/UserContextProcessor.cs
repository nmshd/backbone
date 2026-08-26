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
        data.SetTag("enmeshed.backbone.client_id", _userContext.GetClientIdOrNull() ?? Baggage.Current.GetBaggage("enmeshed.backbone.client_id"));
        data.SetTag("enmeshed.backbone.device_id", _userContext.GetDeviceIdOrNull() ?? Baggage.Current.GetBaggage("enmeshed.backbone.device_id"));
        data.SetTag("enmeshed.backbone.identity_address", _userContext.GetAddressOrNull()?.Value ?? Baggage.Current.GetBaggage("enmeshed.backbone.identity_address"));
        data.SetTag("enmeshed.backbone.username", _userContext.GetUsernameOrNull() ?? Baggage.Current.GetBaggage("enmeshed.backbone.username"));
    }
}
