using Backbone.Modules.Synchronization.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Synchronization.Application.Datawallets.DTOs;

public class CreatedDatawalletModificationDTO : IMapTo<DatawalletModification>
{
    public string Id { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
}
