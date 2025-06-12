using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteQuotaForIdentity;

public class DeleteQuotaForIdentityCommand : IRequest
{
    public required string IdentityAddress { get; init; }
    public required string IndividualQuotaId { get; init; }
}
