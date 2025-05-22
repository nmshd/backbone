namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;

public class DatawalletModification
{
    public string Id { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public string ObjectIdentifier { get; set; } = null!;
    public string Collection { get; set; } = null!;
    public DatawalletModificationType Type { get; set; }
    public string? PayloadCategory { get; set; }
    public byte[]? EncryptedPayload { get; set; }
}

public enum DatawalletModificationType
{
    Create = 0,
    Update = 1,
    Delete = 2,
    CacheChanged = 3
}
