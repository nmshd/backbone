using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using OpenTelemetry;

namespace Backbone.BuildingBlocks.API.Diagnostics;

public sealed class IdentityAddressProcessor : BaseProcessor<Activity>
{
    private readonly IUserContext _userContext;

    public IdentityAddressProcessor(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public override void OnEnd(Activity data)
    {
        data.SetTag("identityAddress", _userContext.GetAddressOrNull()?.Value);
    }
}
