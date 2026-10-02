using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListFeatureFlags;

public class Response : Dictionary<string, bool>
{
    public Response(FeatureFlagSet featureFlags)
    {
        foreach (var featureFlag in featureFlags)
        {
            Add(featureFlag.Name.Value, featureFlag.IsEnabled);
        }
    }
}
