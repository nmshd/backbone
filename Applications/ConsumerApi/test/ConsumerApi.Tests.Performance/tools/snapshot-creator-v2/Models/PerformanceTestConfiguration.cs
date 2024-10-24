using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record PerformanceTestConfiguration(
    [property: JsonPropertyName("Pools")] List<IdentityPoolConfiguration> IdentityPoolConfigs,
    [property: JsonPropertyName("Verification")]
    VerificationConfiguration VerificationConfiguration)
{
    public virtual bool Equals(PerformanceTestConfiguration? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        var isConfigEqual = VerificationConfiguration.App.Equals(other.VerificationConfiguration.App) &&
                            VerificationConfiguration.Connector.Equals(other.VerificationConfiguration.Connector);
        var isPoolConfigsEqual = IdentityPoolConfigs.SequenceEqual(other.IdentityPoolConfigs);


        return isConfigEqual && isPoolConfigsEqual;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(VerificationConfiguration.App);
        hash.Add(VerificationConfiguration.Connector);

        foreach (var poolConfig in IdentityPoolConfigs)
        {
            hash.Add(poolConfig);
        }

        return hash.ToHashCode();
    }
}
