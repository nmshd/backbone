// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Synchronization;

public class DatawalletModification
{
    public required string Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public required string ObjectIdentifier { get; init; }
    public required string Collection { get; init; }
    public required DatawalletModificationType Type { get; init; }
    public required string? PayloadCategory { get; init; }
    public required byte[]? EncryptedPayload { get; init; }
}

public enum DatawalletModificationType
{
    Create = 0,
    Update = 1,
    Delete = 2,
    CacheChanged = 3
}
