using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetFeatureFlags;

public class GetFeatureFlagsResponse : Dictionary<string, bool>
{
    public GetFeatureFlagsResponse(FeatureFlagSet featureFlags)
    {
        foreach (var featureFlag in featureFlags)
        {
            Add(featureFlag.Name.Value, featureFlag.IsEnabled);
        }
    }
}
