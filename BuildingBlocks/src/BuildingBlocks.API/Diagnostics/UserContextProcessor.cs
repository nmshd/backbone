using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;

namespace Backbone.BuildingBlocks.API.Diagnostics;

public sealed class UserContextProcessor : BaseProcessor<Activity>
{
    private readonly IServiceProvider _serviceProvider;

    public UserContextProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override void OnEnd(Activity data)
    {
        var userContext = _serviceProvider.GetService<IUserContext>();

        data.SetTag("enmeshed.backbone.client_id", userContext?.GetClientIdOrNull() ?? Baggage.Current.GetBaggage("enmeshed.backbone.client_id"));
        data.SetTag("enmeshed.backbone.device_id", userContext?.GetDeviceIdOrNull() ?? Baggage.Current.GetBaggage("enmeshed.backbone.device_id"));
        data.SetTag("enmeshed.backbone.identity_address", userContext?.GetAddressOrNull()?.Value ?? Baggage.Current.GetBaggage("enmeshed.backbone.identity_address"));
        data.SetTag("enmeshed.backbone.username", userContext?.GetUsernameOrNull() ?? Baggage.Current.GetBaggage("enmeshed.backbone.username"));
    }
}
