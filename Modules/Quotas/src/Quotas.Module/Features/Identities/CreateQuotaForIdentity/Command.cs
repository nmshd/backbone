using Backbone.Modules.Quotas.Module.Features.Shared;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Identities.CreateQuotaForIdentity;

public class CreateQuotaForIdentityCommand : IRequest<IndividualQuotaDTO>
{
    public required string IdentityAddress { get; init; }
    public required string MetricKey { get; init; }
    public required int Max { get; init; }
    public required QuotaPeriod Period { get; init; }
}
