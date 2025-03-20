namespace Backbone.Modules.Devices.Domain.Entities.Identities;

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
