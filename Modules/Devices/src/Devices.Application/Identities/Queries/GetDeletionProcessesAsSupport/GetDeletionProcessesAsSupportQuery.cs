using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAsSupport;

public class GetDeletionProcessesAsSupportQuery : IRequest<GetDeletionProcessesAsSupportResponse>
{
    public GetDeletionProcessesAsSupportQuery(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }
    public string IdentityAddress { get; set; }
}
