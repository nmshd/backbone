using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.Synchronization.Domain.Entities;

namespace Backbone.Synchronization.Application.Datawallets.DTOs;

public class CreatedDatawalletModificationDTO : IMapTo<DatawalletModification>
{
    public string Id { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
}
