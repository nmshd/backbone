using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Quotas.Application.DTOs;
public class TierDTO : IMapTo<Tier>
{
    public TierDTO(string id, string name, IEnumerable<TierQuotaDefinitionDTO> quotas)
    {
        Id = id;
        Name = name;
        Quotas = quotas;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<TierQuotaDefinitionDTO> Quotas { get; set; }
}