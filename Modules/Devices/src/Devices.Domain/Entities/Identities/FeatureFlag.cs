namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class FeatureFlag
{
    public FeatureFlag(FeatureFlagName name, bool value)
    {
        Name = name;
        IsEnabled = value;
    }

    public bool IsEnabled { get; private set; }
    public FeatureFlagName Name { get; }

    public void Set(bool value)
    {
        IsEnabled = value;
    }
}

public record FeatureFlagName
{
    private FeatureFlagName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static FeatureFlagName Parse(string value)
    {
        return new FeatureFlagName(value);
    }

    public override string ToString()
    {
        return Value;
    }
}
