using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class DatawalletModificationDTO
{
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
        Type = datawalletModification.Type;
        EncryptedPayload = datawalletModification.EncryptedPayload;
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
}
