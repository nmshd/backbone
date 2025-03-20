namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class FeatureFlagSet
{
    private readonly List<FeatureFlag> _featureFlags = [];

    public bool Contains(FeatureFlagName name)
    {
        return _featureFlags.Any(f => f.Name == name);
    }

    public void Set(FeatureFlagName name, bool value)
    {
        if (Contains(name))
        {
            GetFeatureFlag(name).Set(value);
        }
        else
        {
            _featureFlags.Add(new FeatureFlag(name, value));
        }
    }

    public FeatureFlag GetFeatureFlag(FeatureFlagName name)
    {
        return _featureFlags.First(f => f.Name == name);
    }
}
