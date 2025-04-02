using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class FeatureFlag : Entity
{
    // ReSharper disable once UnusedMember.Local
    private FeatureFlag()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Name = null!;
    }

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
        var validationResult = Validate(value);
        if (validationResult != null)
            throw new ArgumentException(validationResult);

        return new FeatureFlagName(value);
    }

    public static string? Validate(string value)
    {
        if (value.Length > MAX_LENGTH)
            return $"Feature flag name cannot be longer than {MAX_LENGTH} characters.";

        return null;
    }

    public override string ToString()
    {
        return Value;
    }
}
