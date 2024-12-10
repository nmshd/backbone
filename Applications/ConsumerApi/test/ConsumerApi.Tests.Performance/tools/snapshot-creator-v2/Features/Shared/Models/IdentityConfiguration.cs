using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record IdentityConfiguration(int Address, IdentityPoolType IdentityPoolType, PoolConfiguration PoolConfiguration)
{
    public List<RelationshipAndMessages> RelationshipAndMessages { get; } = [];

    public bool HasAvailableRelationships => NumberOfRelationships > 0;

    public string PoolAlias => PoolConfiguration.Alias;

    internal int NumberOfRelationships { get; set; } = PoolConfiguration.NumberOfRelationships;

    public int DecrementAvailableRelationships() => NumberOfRelationships == 0 ? throw new InvalidOperationException("No more relationships available") : NumberOfRelationships--;

    public int MessagesToSendPerRelationship => PoolConfiguration.NumberOfRelationships > 0 ? PoolConfiguration.NumberOfSentMessages / PoolConfiguration.NumberOfRelationships : 0;
    public int ModuloSendMessages => PoolConfiguration.NumberOfRelationships > 0 ? PoolConfiguration.NumberOfSentMessages % PoolConfiguration.NumberOfRelationships : 0;
    public int NumberOfSentMessages => PoolConfiguration.NumberOfSentMessages;

    public int NumberOfDevices => PoolConfiguration.NumberOfDevices;
    public int NumberOfRelationshipTemplates => PoolConfiguration.NumberOfRelationshipTemplates;
    public int NumberOfChallenges => PoolConfiguration.NumberOfChallenges;
    public int NumberOfDatawalletModifications => PoolConfiguration.NumberOfDatawalletModifications;

    public void IncrementAvailableRelationships()
    {
        if (NumberOfRelationships == PoolConfiguration.NumberOfRelationships)
        {
            throw new InvalidOperationException("Can't add more relationships than configured");
        }

        NumberOfRelationships++;
    }
}
