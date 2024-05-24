using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class CreatedDatawalletModificationDTO : IMapTo<DatawalletModification>
{
    public required DatawalletModificationId Id { get; set; }
    public required long Index { get; set; }
    public required DateTime CreatedAt { get; set; }
}
