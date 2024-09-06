using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.PerformanceSnapshotCreator.Application.MessageDistributor;
using Backbone.PerformanceSnapshotCreator.Application.Printer;
using Backbone.PerformanceSnapshotCreator.Application.RelationshipDistributor;
using Backbone.PerformanceSnapshotCreator.Domain;
using Backbone.PerformanceSnapshotCreator.PoolsFile;
using Backbone.Tooling;

namespace Backbone.PerformanceSnapshotCreator.PoolsGenerator;

public class DeterministicPoolsGenerator
{
    private readonly IRelationshipDistributor _relationshipDistributor;
    private readonly IMessageDistributor _messageDistributor;
    private readonly IPrinter _printer;
    private List<PoolEntry> _pools;
    private readonly PoolsOffset _poolsOffset;
    private readonly PoolFileRoot _config;

    public DeterministicPoolsGenerator(
        PoolFileRoot configuration,
        IRelationshipDistributor relationshipDistributor,
        IMessageDistributor messageDistributor,
        IPrinter printer)
    {
        _relationshipDistributor = relationshipDistributor;
        _messageDistributor = messageDistributor;
        _printer = printer;
        _pools = configuration.Pools.ToList();
        _config = configuration;

        CalculateSentAndReceivedMessages(_pools, configuration.Configuration);

        _poolsOffset = PoolsOffset.CalculatePoolOffsets(_pools.ToArray());
    }

    public void CreatePools()
    {
        _pools = PoolsOffset.CreateOffsetPools(_pools, _poolsOffset);

        CheckPoolsConfiguration();
        CreateFakeIdentities();

        EstablishMessagesOffsetPoolsRelationships(_pools);

        _relationshipDistributor.Distribute(_pools);
        _printer.PrintRelationships(_pools, summaryOnly: true);
        _messageDistributor.Distribute(_pools);

        _printer.PrintRelationships(_pools, summaryOnly: true);
        Console.WriteLine($"Removed {TrimUnusedRelationships()} unused relationships.");

        _printer.PrintRelationships(_pools, summaryOnly: true);
        _printer.PrintMessages(_pools, summaryOnly: true);

        _config.Pools = _pools.ToArray();
    }

    private uint TrimUnusedRelationships()
    {
        var allIdentities = _pools.SelectMany(p => p.Identities);

        uint removedCount = 0;

        foreach (var identity in allIdentities)
        {
            for (var i = 0; i < identity.IdentitiesToEstablishRelationshipsWith.Count; i++)
            {
                var relationshipIdentity = identity.IdentitiesToEstablishRelationshipsWith[i];
                if (!identity.IdentitiesToSendMessagesTo.Contains(relationshipIdentity))
                {
                    identity.IdentitiesToEstablishRelationshipsWith.Remove(relationshipIdentity);
                    removedCount++;
                    i--;
                }
            }
        }

        return removedCount;
    }

    /// <summary>
    /// For each pool p, p.NumberOfRelationships must NOT be greater than the sum of identities in all pools of the opposite type
    /// </summary>
    private void CheckPoolsConfiguration()
    {
        var expectedAppIdentitiesCount = _pools.Where(p => p.IsApp()).Sum(p => p.Amount);
        var expectedConnectorIdentitiesCount = _pools.Where(p => p.IsConnector()).Sum(p => p.Amount);

        foreach (var poolEntry in _pools)
        {
            var oppositeIdentitiesCount = poolEntry.IsApp() ? expectedConnectorIdentitiesCount : expectedAppIdentitiesCount;

            if (poolEntry.NumberOfRelationships > oppositeIdentitiesCount)
            {
                throw new Exception($"The number of relationships ({poolEntry.NumberOfRelationships}) for pool {poolEntry.Name} is higher than the number of identities in the opposite pools.");
            }
        }
    }

    private void CreateFakeIdentities()
    {
        uint uon = 0;
        foreach (var pool in _pools)
        {
            for (uint i = 0; i < pool.Amount; i++)
            {
                var createdIdentity = new Identity(new UserCredentials("USR" + PasswordHelper.GeneratePassword(8, 8), PasswordHelper.GeneratePassword(18, 24)),
                    "ID1" + PasswordHelper.GeneratePassword(16, 16), "DVC" + PasswordHelper.GeneratePassword(8, 8), pool, i + 1, uon++);

                if (pool.NumberOfDevices > 1)
                {
                    for (uint j = 1; j < pool.NumberOfDevices; j++)
                        createdIdentity.AddDevice("DVC" + PasswordHelper.GeneratePassword(8, 8));
                }

                pool.Identities.Add(createdIdentity);
            }
        }
    }

    private static void CalculateSentAndReceivedMessages(IList<PoolEntry> pools, PoolFileConfiguration poolsConfiguration)
    {
        var messagesSentByConnectorRatio = poolsConfiguration.MessagesSentByConnectorRatio;
        var messagesSentByAppRatio = 1 - messagesSentByConnectorRatio;

        foreach (var pool in pools.Where(p => p.TotalNumberOfMessages > 0))
        {
            if (pool.IsApp())
            {
                pool.NumberOfSentMessages = Convert.ToUInt32(decimal.Ceiling(pool.TotalNumberOfMessages * messagesSentByAppRatio));
            }
            else if (pool.IsConnector())
            {
                pool.NumberOfSentMessages = Convert.ToUInt32(decimal.Ceiling(pool.TotalNumberOfMessages * messagesSentByConnectorRatio));
            }
            else
            {
                throw new Exception("Pools that are neither app nor connector cannot send messages.");
            }

            pool.NumberOfReceivedMessages = pool.TotalNumberOfMessages - pool.NumberOfSentMessages;

            if (pool.NumberOfReceivedMessages == 0 || pool.NumberOfSentMessages == 0)
            {
                throw new Exception(
                    $"The resulting number of sent/received messages for pool {pool.Name} is zero. Please use a higher number and/or adjust the ratio. Otherwise, the number of messages will not match.");
            }
        }
    }

    private static void EstablishMessagesOffsetPoolsRelationships(IList<PoolEntry> pools)
    {
        var messagesOffsetPools = pools.Where(p => p.Alias.StartsWith("a0m") || p.Alias.StartsWith("c0m")).ToList();
        if (messagesOffsetPools.Count != 2) return;

        var (p1, p2) = (messagesOffsetPools[0], messagesOffsetPools[1]);
        p1.Identities.Single().AddIdentityToEstablishRelationshipsWith(p2.Identities.Single());
    }
}
