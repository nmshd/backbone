using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListDeletionProcessesAsSupport;

public class ListDeletionProcessesAsSupportQuery : IRequest<GetDeletionProcessesAsSupportResponse>
{
    public ListDeletionProcessesAsSupportQuery(string identityAddress)
    {
        IdentityAddress = identityAddress;
    }

    public string IdentityAddress { get; set; }
}
