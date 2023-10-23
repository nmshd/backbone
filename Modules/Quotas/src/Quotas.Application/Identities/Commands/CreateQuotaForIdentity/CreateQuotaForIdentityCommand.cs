using Backbone.Quotas.Application.DTOs;
using Backbone.Quotas.Domain.Aggregates.Identities;
using MediatR;

namespace Backbone.Quotas.Application.Identities.Commands.CreateQuotaForIdentity;

public class CreateQuotaForIdentityCommand : IRequest<IndividualQuotaDTO>
{
    public CreateQuotaForIdentityCommand(string identityAddress, string metricKey, int max, QuotaPeriod period)
    {
        IdentityAddress = identityAddress;
        MetricKey = metricKey;
        Max = max;
        Period = period;
    }

    public string IdentityAddress { get; set; }
    public string MetricKey { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}
