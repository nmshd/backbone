using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class DatawalletModificationDTO
{
    public enum DatawalletModificationType
    {
        Create,
        Update,
        Delete,
        CacheChanged
    }

    public DatawalletModificationDTO(DatawalletModification datawalletModification)
    {
        Id = datawalletModification.Id;
        DatawalletVersion = datawalletModification.DatawalletVersion;
        Index = datawalletModification.Index;
        ObjectIdentifier = datawalletModification.ObjectIdentifier;
        PayloadCategory = datawalletModification.PayloadCategory;
        CreatedAt = datawalletModification.CreatedAt;
        CreatedByDevice = datawalletModification.CreatedByDevice;
        Collection = datawalletModification.Collection;
        Type = MapDatawalletModificationType(datawalletModification.Type);
        EncryptedPayload = datawalletModification.Details.EncryptedPayload;
    }

    public string Id { get; set; }
    public ushort DatawalletVersion { get; set; }
    public long Index { get; set; }
    public string ObjectIdentifier { get; set; }
    public string? PayloadCategory { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByDevice { get; set; }
    public string Collection { get; set; }
    public DatawalletModificationType Type { get; set; }
    public byte[]? EncryptedPayload { get; set; }

    private static DatawalletModificationType MapDatawalletModificationType(Domain.Entities.DatawalletModificationType type)
    {
        return type switch
        {
            Domain.Entities.DatawalletModificationType.Create => DatawalletModificationType.Create,
            Domain.Entities.DatawalletModificationType.Update => DatawalletModificationType.Update,
            Domain.Entities.DatawalletModificationType.Delete => DatawalletModificationType.Delete,
            Domain.Entities.DatawalletModificationType.CacheChanged => DatawalletModificationType.CacheChanged,
            _ => throw new Exception($"Unsupported Datawallet Modification Type: {type}")
        };
    }
}
