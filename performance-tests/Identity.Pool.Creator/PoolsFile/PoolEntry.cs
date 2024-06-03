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
    public uint NumberOfSentMessages { get; set; }
    public uint NumberOfDatawalletModifications { get; set; }
    public uint NumberOfDevices { get; set; }
    public uint NumberOfChallenges { get; set; }

    public List<Identity> identities = [];
}
