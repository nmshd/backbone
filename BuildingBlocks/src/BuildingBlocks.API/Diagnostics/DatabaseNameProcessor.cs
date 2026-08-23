using System.Diagnostics;
using OpenTelemetry;

namespace Backbone.BuildingBlocks.API.Diagnostics;

public sealed class DatabaseNameProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity activity)
    {
        if (activity.Source.Name.Contains("EntityFrameworkCore"))
            activity.SetTag("peer.service", "postgres");
    }
}
