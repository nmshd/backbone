using System.Text;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.Identity.Pool.Creator.Application.MessageDistributor;
using Backbone.Identity.Pool.Creator.Application.Printer;
using Backbone.Identity.Pool.Creator.Application.RelationshipDistributor;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Identity.Pool.Creator.Tools;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;

namespace Backbone.Identity.Pool.Creator.PoolsGenerator;
public class PoolsGenerator
{
    private readonly IRelationshipDistributor _relationshipDistributor;
    private readonly IMessageDistributor _messageDistributor;
    private readonly IPrinter _printer;
    private List<PoolEntry> _pools;
    private readonly PoolsOffset _poolsOffset;
    private readonly PoolFileRoot _config;

    public PoolsGenerator(
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

        MessageDistributorTools.CalculateSentAndReceivedMessages(_pools, configuration.Configuration);
        _poolsOffset = PoolsOffset.CalculatePoolOffsets(_pools.ToArray());
    }

    public async Task CreatePools()
    {
        _pools = PoolsOffset.CreateOffsetPools(_pools, _poolsOffset);

        CheckPoolsConfiguration();

        //await CreateIdentities();
        CreateFakeIdentities();

        RelationshipDistributorTools.EstablishMessagesOffsetPoolsRelationships(_pools);

        _relationshipDistributor.Distribute(_pools);
        _printer.PrintRelationships(_pools, summaryOnly: true);
        _messageDistributor.Distribute(_pools);

        _printer.PrintRelationships(_pools, summaryOnly: true);
        Console.WriteLine($"Removed {TrimUnusedRelationships()} unused relationships.");

        _printer.PrintRelationships(_pools, summaryOnly: true);
        _printer.PrintMessages(_pools, summaryOnly: true);

        //await CreateRelationshipTemplates();
        //await CreateChallenges();
        //await CreateMessages();
        //await CreateDatawalletModifications();

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
    /// <exception cref="NotImplementedException"></exception>
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
                var createdIdentity = new Identity(new("USR" + PasswordHelper.GeneratePassword(8, 8), PasswordHelper.GeneratePassword(18, 24)), "ID1" + PasswordHelper.GeneratePassword(16, 16), "DVC" + PasswordHelper.GeneratePassword(8, 8), pool, i+1, uon++);

                if (pool.NumberOfDevices > 1)
                {
                    for (uint j = 1; j < pool.NumberOfDevices; j++)
                        createdIdentity.AddDevice("DVC" + PasswordHelper.GeneratePassword(8, 8));
                }
                pool.Identities.Add(createdIdentity);
            }
        }
    }
}
