using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Quotas.Commands.CreateQuotaForTier;

public class CreateQuotaForTierCommand : IRequest<CreateQuotaForTierResponse>
{
    public string TierId { get; set; }
    public Metric Metric { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}
