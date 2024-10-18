using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record PerformanceTestConfiguration(
    [property: JsonPropertyName("Pools")] List<IdentityPoolConfiguration> IdentityPoolConfigs,
    [property: JsonPropertyName("Configuration")]
    Configuration Configuration)
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

        var isConfigEqual = Configuration.App.Equals(other.Configuration.App) &&
                            Configuration.Connector.Equals(other.Configuration.Connector);
        var isPoolConfigsEqual = IdentityPoolConfigs.SequenceEqual(other.IdentityPoolConfigs);


        return isConfigEqual && isPoolConfigsEqual;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Configuration.App);
        hash.Add(Configuration.Connector);

        foreach (var poolConfig in IdentityPoolConfigs)
        {
            hash.Add(poolConfig);
        }

        return hash.ToHashCode();
    }
}
