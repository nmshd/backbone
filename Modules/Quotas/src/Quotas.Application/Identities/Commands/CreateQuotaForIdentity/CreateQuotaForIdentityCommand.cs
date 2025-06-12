using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.CreateQuotaForIdentity;

public class CreateQuotaForIdentityCommand : IRequest<IndividualQuotaDTO>
{
    public required string IdentityAddress { get; init; }
    public required string MetricKey { get; init; }
    public required int Max { get; init; }
    public required QuotaPeriod Period { get; init; }
}
