using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class FeatureFlag : Entity
{
    public FeatureFlag(FeatureFlagName name, bool isEnabled)
    {
        Name = name;
        IsEnabled = isEnabled;
    }

    public bool IsEnabled { get; private set; }
    public FeatureFlagName Name { get; }
    public IdentityAddress OwnerAddress { get; } = null!;

    public void Set(bool value)
    {
        IsEnabled = value;
    }
}

public record FeatureFlagName
{
    public const int MAX_LENGTH = 200;

    private FeatureFlagName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static FeatureFlagName Parse(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"Feature flag name cannot be longer than {MAX_LENGTH} characters.");

        return new FeatureFlagName(value);
    }

    public static bool IsValid(string value)
    {
        return value.Length <= MAX_LENGTH;
    }

    public override string ToString()
    {
        return Value;
    }
}
