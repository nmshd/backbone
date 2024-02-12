﻿using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.ListQuotasForIdentity;

public class ListQuotasForIdentityResponse : CollectionResponseBase<QuotaGroupDTO>
{
    public ListQuotasForIdentityResponse(IEnumerable<QuotaGroupDTO> items) : base(items) { }
}

public class SingleQuotaDTO
{
    public QuotaSource Source { get; set; }
    public string MetricKey { get; set; }
    public int Max { get; set; }
    public uint Usage { get; set; }
    public string Period { get; set; }
}

public class QuotaGroupDTO
{
    public string MetricKey { get; set; }
    public List<SingleQuotaDTO> Quotas { get; set; }
}
