using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

public class PushDatawalletModificationsResponse
{
    public required long NewIndex { get; set; }

    public required IEnumerable<PushDatawalletModificationsResponseItem> Modifications { get; set; }
}

public class PushDatawalletModificationsResponseItem : IMapTo<DatawalletModification>
{
    public required DatawalletModificationId Id { get; set; }
    public required long Index { get; set; }
    public required DateTime CreatedAt { get; set; }
}
