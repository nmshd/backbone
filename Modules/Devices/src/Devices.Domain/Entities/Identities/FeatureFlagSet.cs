using System.Collections;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class FeatureFlagSet : ICollection<FeatureFlag>
{
    private readonly List<FeatureFlag> _featureFlags = [];
    public HashSet<FeatureFlagName> Names => [.. _featureFlags.Select(f => f.Name)];
    
    public static FeatureFlagSet Load(List<FeatureFlag> featureFlags)
    {
        var featureFlagSet = new FeatureFlagSet();
        foreach (var featureFlag in featureFlags)
        {
            featureFlagSet.Add(featureFlag);
        }

        return featureFlagSet;
    }

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

    public IEnumerator<FeatureFlag> GetEnumerator()
    {
        return _featureFlags.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // TODO: try to make this pretty
    public void Add(FeatureFlag item)
    {
        Set(item.Name, item.IsEnabled);
    }

    public void Clear()
    {
        _featureFlags.Clear();
    }

    public bool Contains(FeatureFlag item)
    {
        return _featureFlags.Contains(item);
    }

    public void CopyTo(FeatureFlag[] array, int arrayIndex)
    {
        _featureFlags.CopyTo(array, arrayIndex);
    }

    public bool Remove(FeatureFlag item)
    {
        return _featureFlags.Remove(item);
    }

    public int Count => _featureFlags.Count;
    public bool IsReadOnly => false;
}
