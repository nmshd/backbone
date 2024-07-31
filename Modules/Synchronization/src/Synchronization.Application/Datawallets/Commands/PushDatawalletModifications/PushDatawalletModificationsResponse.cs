using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

public class PushDatawalletModificationsResponse
{
    public required long NewIndex { get; set; }

    public required IEnumerable<PushDatawalletModificationsResponseItem> Modifications { get; set; }
}

public class PushDatawalletModificationsResponseItem
{
    public PushDatawalletModificationsResponseItem(DatawalletModification datawalletModification)
    {
        Id = datawalletModification.Id;
        Index = datawalletModification.Index;
        CreatedAt = datawalletModification.CreatedAt;
    }

    public string Id { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
}
