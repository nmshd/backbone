using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Module.Features.Identities.ListQuotasForIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListQuotasForIdentityResponse")]
public class Response : CollectionResponseBase<QuotaGroupDTO>
{
    public Response(IEnumerable<QuotaGroupDTO> items) : base(items) { }
}

public class SingleQuotaDTO
{
    public required QuotaSource Source { get; set; }
    public required string MetricKey { get; set; }
    public required int Max { get; set; }
    public required uint Usage { get; set; }
    public required string Period { get; set; }
}

public class QuotaGroupDTO
{
    public required string MetricKey { get; set; }
    public required List<SingleQuotaDTO> Quotas { get; set; }
}
