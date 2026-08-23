using System.Diagnostics;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using OpenTelemetry;

namespace Backbone.BuildingBlocks.API.Diagnostics;

public sealed class UsernameProcessor : BaseProcessor<Activity>
{
    private readonly IUserContext _userContext;

    public UsernameProcessor(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public override void OnEnd(Activity data)
    {
        data.SetTag("username", _userContext.GetUsernameOrNull());
    }
}
