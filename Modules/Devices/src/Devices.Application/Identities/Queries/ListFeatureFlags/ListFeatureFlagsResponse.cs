using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListFeatureFlags;

public class ListFeatureFlagsResponse : Dictionary<string, bool>
{
    public ListFeatureFlagsResponse(FeatureFlagSet featureFlags)
    {
        foreach (var featureFlag in featureFlags)
        {
            Add(featureFlag.Name.Value, featureFlag.IsEnabled);
        }
    }
}
