using System.Text.Json.Serialization;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record PerformanceTestConfiguration(
    [property: JsonPropertyName("Pools")] List<PoolConfiguration> PoolConfigurations,
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

        var isConfigEqual = VerificationConfiguration.Equals(other.VerificationConfiguration);
        var isPoolConfigsEqual = PoolConfigurations.SequenceEqual(other.PoolConfigurations);
        var isRelationshipAndMessagesEqual = RelationshipAndMessages.SequenceEqual(other.RelationshipAndMessages);

        return isConfigEqual && isPoolConfigsEqual && isRelationshipAndMessagesEqual;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(VerificationConfiguration);

        foreach (var poolConfig in PoolConfigurations)
        {
            hash.Add(poolConfig);
        }

        foreach (var relationshipAndMessage in RelationshipAndMessages)
        {
            hash.Add(relationshipAndMessage);
        }

        return hash.ToHashCode();
    }

    [JsonPropertyName("RelationshipAndMessages")]
    public List<RelationshipAndMessages> RelationshipAndMessages { get; init; } = [];


    [JsonIgnore] public bool IsIdentityPoolConfigurationCreated { get; private set; }

    [JsonIgnore] public List<IdentityPoolConfiguration> IdentityPoolConfigurations { get; private set; } = [];

    public List<IdentityPoolConfiguration> CreateIdentityPoolConfigurations()
    {
        if (IsIdentityPoolConfigurationCreated) return IdentityPoolConfigurations;

        IdentityPoolConfigurations = PoolConfigurations
            .Select(poolConfiguration => new IdentityPoolConfiguration(poolConfiguration))
            .ToList();

        IsIdentityPoolConfigurationCreated = true;
        return IdentityPoolConfigurations;
    }
}
