using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Quotas.Application.DTOs;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.ListQuotasForIdentity;

public class ListQuotasForIdentityResponse : EnumerableResponseBase<QuotaDTO>
{
    public ListQuotasForIdentityResponse(IEnumerable<QuotaDTO> items) : base(items) { }
}
