using Backbone.Modules.Quotas.Application.DTOs;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;
public class GetTierByIdResponse : TierDTO
{
    public GetTierByIdResponse(string id, string name, IEnumerable<TierQuotaDefinitionDTO> quotas) : base(id, name, quotas) { }

}