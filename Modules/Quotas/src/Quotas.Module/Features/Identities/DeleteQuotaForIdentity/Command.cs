using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Identities.DeleteQuotaForIdentity;

public class DeleteQuotaForIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
    public required string IndividualQuotaId { get; init; }
}
