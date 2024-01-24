using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.ListQuotasForIdentity;

public class ListQuotasForIdentityResponse : CollectionResponseBase<QuotaGroupDTO>
{
    public ListQuotasForIdentityResponse(IEnumerable<QuotaGroupDTO> items) : base(items) { }
}

public class SingleQuotaDTO
{
    public QuotaSource Source { get; set; }
    public required string MetricKey { get; set; }
    public int Max { get; set; }
    public uint Usage { get; set; }
    public required string Period { get; set; }
}

public class QuotaGroupDTO
{
    public required string MetricKey { get; set; }
    public required List<SingleQuotaDTO> Quotas { get; set; }
}
