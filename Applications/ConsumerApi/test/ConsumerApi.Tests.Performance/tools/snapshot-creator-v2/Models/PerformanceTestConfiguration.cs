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

        var isConfigEqual = VerificationConfiguration.Equals(other.VerificationConfiguration);
        var isPoolConfigsEqual = IdentityPoolConfigs.SequenceEqual(other.IdentityPoolConfigs);
        var isRelationshipAndMessagesEqual = RelationshipAndMessages.SequenceEqual(other.RelationshipAndMessages);

        return isConfigEqual && isPoolConfigsEqual && isRelationshipAndMessagesEqual;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(VerificationConfiguration);

        foreach (var poolConfig in IdentityPoolConfigs)
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
}
