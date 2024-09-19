using Backbone.Modules.Devices.Application.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsSupport;
public class GetDeletionProcessAsSupportQuery : IRequest<IdentityDeletionProcessDetailsDTO>
{
    public GetDeletionProcessAsSupportQuery(string identityAddress, string deletionProcessId)
    {
        IdentityAddress = identityAddress;
        DeletionProcessId = deletionProcessId;
    }

    public string IdentityAddress { get; set; }
    public string DeletionProcessId { get; set; }
}
