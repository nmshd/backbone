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

        if (RelationshipAndMessages.Count == 0 && other.RelationshipAndMessages.Count == 0)
        {
            return isConfigEqual && isPoolConfigsEqual;
        }

        var a1PoolRelations = RelationshipAndMessages.Where(r => r.SenderPoolAlias == "a1").ToList();
        var a2PoolRelations = RelationshipAndMessages.Where(r => r.SenderPoolAlias == "a2").ToList();
        var a3PoolRelations = RelationshipAndMessages.Where(r => r.SenderPoolAlias == "a3").ToList();

        var a1PoolConfig = PoolConfigurations.First(p => p.Alias == "a1");
        var a2PoolConfig = PoolConfigurations.First(p => p.Alias == "a2");
        var a3PoolConfig = PoolConfigurations.First(p => p.Alias == "a3");

        var isA1PoolValid = a1PoolRelations.Count == a1PoolConfig.NumberOfRelationships;
        var isA2PoolValid = a2PoolRelations.Count == a2PoolConfig.NumberOfRelationships * a2PoolConfig.Amount;
        var isA3PoolValid = a3PoolRelations.Count == a3PoolConfig.NumberOfRelationships * a3PoolConfig.Amount;

        var otherA1PoolRelations = other.RelationshipAndMessages.Where(r => r.SenderPoolAlias == "a1").ToList();
        var otherA2PoolRelations = other.RelationshipAndMessages.Where(r => r.SenderPoolAlias == "a2").ToList();
        var otherA3PoolRelations = other.RelationshipAndMessages.Where(r => r.SenderPoolAlias == "a3").ToList();

        var isOtherA1PoolValid = otherA1PoolRelations.Count == a1PoolConfig.NumberOfRelationships;
        var isOtherA2PoolValid = otherA2PoolRelations.Count == a2PoolConfig.NumberOfRelationships * a2PoolConfig.Amount;
        var isOtherA3PoolValid = otherA3PoolRelations.Count == a3PoolConfig.NumberOfRelationships * a3PoolConfig.Amount;

        var isRelationshipAndMessagesEqual = RelationshipAndMessages.Count == other.RelationshipAndMessages.Count &&
                                             isA1PoolValid && isA2PoolValid && isA3PoolValid &&
                                             isOtherA1PoolValid && isOtherA2PoolValid && isOtherA3PoolValid;

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
