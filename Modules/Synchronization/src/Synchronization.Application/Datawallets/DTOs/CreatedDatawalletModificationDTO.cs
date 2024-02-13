using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class CreatedDatawalletModificationDTO : IMapTo<DatawalletModification>
{
    public required string Id { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
}
