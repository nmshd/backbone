using System.Text.Json.Serialization;
using NSec.Cryptography;

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

    [JsonIgnore]
    public PoolEntry Pool = null!;

    public bool IsConnector() => Type == PoolTypes.CONNECTOR_TYPE;
    public bool IsApp() => Type == PoolTypes.APP_TYPE;
}

public static class PoolEntryExtensionMethods{
    public static bool HasMessagesOffsetPool(this IList<PoolEntry> pools)
    {
        return pools.Any(p => p.Alias.EndsWith("0mc"));
    }
}
