using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Identities.CreateQuotaForIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateQuotaForIdentityCommand")]
public class Command : IRequest<IndividualQuotaDTO>
{
    public required string IdentityAddress { get; init; }
    public required string MetricKey { get; init; }
    public required int Max { get; init; }
    public required QuotaPeriod Period { get; init; }
}
