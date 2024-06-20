using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.Identity.Pool.Creator.Application.MessageDistributor;
using Backbone.Identity.Pool.Creator.Application.Printer;
using Backbone.Identity.Pool.Creator.Application.RelationshipDistributor;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Tooling;

namespace Backbone.Identity.Pool.Creator.GraphPoolsGenerator;

/// <summary>
/// We use a different approach here. Instead of creating all identities, then their relationships and lastly their messages,
/// instead we create a handful of identities first along with some relationships and messages. This is organised into a graph
/// where relationships are represented by edges, and messages are represented by weighted edges.
/// 
/// After this initial setup, the allocation of messages is done using a mix of relationship creation, pre-existing relationship usage
/// and new identity creation, as per the limits set in the pool configuration.
/// </summary>
public class GraphPoolsGenerator
{
    private readonly string _baseAddress;
    private readonly ClientCredentials _clientCredentials;
    private readonly IPrinter _printer;
    private readonly List<PoolEntry> _pools;

    public GraphPoolsGenerator(
        string baseAddress,
        string clientId,
        string clientSecret,
        PoolFileRoot configuration,
        IPrinter printer)
    {
        _baseAddress = baseAddress;
        _clientCredentials = new ClientCredentials(clientId, clientSecret);
        _printer = printer;
        _pools = configuration.Pools.ToList();
    }

    public async Task CreatePools()
    {
        CreateInitialNodes();
        _printer.PrintRelationships(_pools, summaryOnly: false);
        _printer.PrintMessages(_pools, summaryOnly: true);

        DeepGenerate();

        _printer.PrintRelationships(_pools, summaryOnly: false);
        _printer.PrintMessages(_pools, summaryOnly: true);
    }

    private void DeepGenerate()
    {
        Console.WriteLine($"Got called with {_pools.NumberOfEstablishedRelationships()} relationships already established and {_pools.NumberOfSentMessages()} messages sent.");

        var enm = _pools.ExpectedNumberOfSentMessages();
        var enr = _pools.ExpectedNumberOfRelationships();

        Console.WriteLine($"Targets are {enr} relationships and {enm} messages sent.");

        //SendMessagesRecursive();        
        
    }

    private void SendMessagesRecursive(Identity i)
    {
        var relatedIdentities = i.IdentitiesToEstablishRelationshipsWith;

    }

    private void CreateInitialNodes()
    {
        // create a single identity for each pool
        foreach (var poolEntry in _pools.Where(p => p.Amount > 0))
        {
            poolEntry.Identities.Add(new Identity(new("USR" + PasswordHelper.GeneratePassword(8, 8), PasswordHelper.GeneratePassword(18, 24)), "ID1" + PasswordHelper.GeneratePassword(16, 16), "DVC" + PasswordHelper.GeneratePassword(8, 8), poolEntry, 0));
        }

        var appPools = _pools.Where(p => p.IsApp() && p.NumberOfRelationships > 0).ToList();
        var connectorPools = _pools.Where(p => p.IsConnector() && p.NumberOfRelationships > 0).ToList();

        foreach (var appPool in appPools)
        {
            var connectorIdentityWithCapacityForRelationships = connectorPools.SelectMany(p => p.Identities).Where(i => i.RelationshipsCapacity > 0 && i.IdentitiesToEstablishRelationshipsWith.Count == 0).FirstOrDefault();
            if (connectorIdentityWithCapacityForRelationships is not null) 
                appPool.Identities.First().AddIdentityToEstablishRelationshipsWith(connectorIdentityWithCapacityForRelationships);
        }
    }
}
