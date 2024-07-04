using System.Text.Json.Serialization;

namespace Backbone.Identity.Pool.Creator.PoolsFile;
public record PoolEntry
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string Alias { get; set; }

    /// <summary>
    /// The number of <strong>Identities</strong> to be created in this pool.
    /// </summary>
    public uint Amount { get; set; }

    public uint NumberOfRelationshipTemplates { get; set; } = 0;
    public uint NumberOfRelationships { get; set; } = 0;
    public uint TotalNumberOfMessages { get; set; } = 0;
    public uint NumberOfDatawalletModifications { get; set; } = 0;
    public uint NumberOfDevices { get; set; } = 0;
    public uint NumberOfChallenges { get; set; } = 0;

    [JsonIgnore]
    public List<Identity> Identities = [];

    [JsonIgnore] public uint NumberOfSentMessages;

    [JsonIgnore] public uint NumberOfReceivedMessages;

    public bool IsConnector() => Type == PoolTypes.CONNECTOR_TYPE;
    public bool IsApp() => Type == PoolTypes.APP_TYPE;
}

public static class PoolEntryExtensionMethods
{
    public static bool HasMessagesOffsetPool(this IList<PoolEntry> pools)
    {
        return pools.Any(p => p.Alias.EndsWith("0mc"));
    }

    public static long ExpectedNumberOfRelationships(this IList<PoolEntry> pools, bool noError = false)
    {
        var appRelationshipsCount = pools.Where(p => p.IsApp()).Sum(p => p.NumberOfRelationships * p.Amount);
        var connectorRelationshipsCount = pools.Where(p => p.IsConnector()).Sum(p => p.NumberOfRelationships * p.Amount);

        if (noError && appRelationshipsCount != connectorRelationshipsCount)
            return -1;

        return appRelationshipsCount > connectorRelationshipsCount ? appRelationshipsCount : connectorRelationshipsCount;
    }

    public static long ExpectedNumberOfSentMessages(this IList<PoolEntry> pools)
    {
        var appMessagesCount = pools.Where(p => p.IsApp()).Sum(p => p.NumberOfSentMessages * p.Amount);
        var connectorMessageCount = pools.Where(p => p.IsConnector()).Sum(p => p.NumberOfSentMessages * p.Amount);

        return appMessagesCount + connectorMessageCount;
    }

    public static long NumberOfSentMessages(this IList<PoolEntry> pools)
    {
        var appMessagesCount = pools.Where(p => p.IsApp()).SelectMany(p => p.Identities).Sum(i => i.IdentitiesToSendMessagesTo.Count);
        var connectorMessageCount = pools.Where(p => p.IsConnector()).SelectMany(p => p.Identities).Sum(i => i.IdentitiesToSendMessagesTo.Count);

        return appMessagesCount + connectorMessageCount;
    }

    public static long NumberOfEstablishedRelationships(this IList<PoolEntry> pools)
    {
        return pools.SelectMany(p => p.Identities).SelectMany(i => i.IdentitiesToEstablishRelationshipsWith).Distinct().Count() / 2;
    }

    public static IList<PoolEntry> GetAppPools(this IList<PoolEntry> pools)
    {
        return pools.Where(p => p.IsApp() && p.NumberOfRelationships > 0).ToList();
    }

    public static IList<PoolEntry> GetConnectorPools(this IList<PoolEntry> pools)
    {
        return pools.Where(p => p.IsConnector() && p.NumberOfRelationships > 0).ToList();
    }

    public static void AddUonsIfMissing(this IList<PoolEntry> pools)
    {
        if (pools.SelectMany(p => p.Identities).Select(i => i.Uon).Distinct().Count() == 1)
        {
            uint i = 0;
            foreach (var identity in pools.SelectMany(p => p.Identities))
            {
                identity.Uon = ++i;
            }
        }
    }
}
