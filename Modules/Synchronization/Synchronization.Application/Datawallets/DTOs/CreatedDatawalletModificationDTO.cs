using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Synchronization.Domain.Entities;

namespace Synchronization.Application.Datawallets.DTOs;

public class CreatedDatawalletModificationDTO : IMapTo<DatawalletModification>
{
    public string Id { get; set; }
    public long Index { get; set; }
    public DateTime CreatedAt { get; set; }
}
