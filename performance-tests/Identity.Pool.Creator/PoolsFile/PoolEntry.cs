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
    public uint NumberOfRelationshipTemplates { get; set; }
    public uint NumberOfRelationships { get; set; }
    public uint TotalNumberOfMessages { get; set; }
    public uint NumberOfDatawalletModifications { get; set; }
    public uint NumberOfDevices { get; set; }
    public uint NumberOfChallenges { get; set; }

    [JsonIgnore]
    public List<Identity> Identities = [];

    [JsonIgnore] public uint NumberOfSentMessages;

    [JsonIgnore] public uint NumberOfReceivedMessages;

    [JsonIgnore]
    private const string CONNECTOR_TYPE = "Connector";

    [JsonIgnore]
    private const string APP_TYPE = "App";
    public bool IsConnector() => Type == CONNECTOR_TYPE;
    public bool IsApp() => Type == APP_TYPE;
}
