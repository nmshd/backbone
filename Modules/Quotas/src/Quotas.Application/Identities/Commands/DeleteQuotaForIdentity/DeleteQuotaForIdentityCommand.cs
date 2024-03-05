using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Commands.DeleteQuotaForIdentity;

public class DeleteQuotaForIdentityCommand : IRequest
{
    public DeleteQuotaForIdentityCommand(string identityAddress, string individualQuotaId)
    {
        IdentityAddress = identityAddress;
        IndividualQuotaId = individualQuotaId;
    }

    public string IdentityAddress { get; set; }
    public string IndividualQuotaId { get; set; }
}
