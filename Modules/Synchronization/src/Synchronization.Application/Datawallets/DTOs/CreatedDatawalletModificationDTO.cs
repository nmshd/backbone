using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class CreatedDatawalletModificationDTO
{
    public CreatedDatawalletModificationDTO(DatawalletModification datawalletModification)
    {
        Id = datawalletModification.Id;
        Index = datawalletModification.Index;
        CreatedAt = datawalletModification.CreatedAt;
    }

    public string Id { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
}
