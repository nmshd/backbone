using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Quotas.Commands.CreateQuotaForTier;

public class CreateQuotaForTierCommand : IRequest<CreateQuotaForTierResponse>
{
    public CreateQuotaForTierCommand(string tierId, Metric metric, int max, QuotaPeriod period)
    {
        TierId = tierId;
        Metric = metric;
        Max = max;
        Period = period;
    }

    public string TierId { get; set; }
    public Metric Metric { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}
