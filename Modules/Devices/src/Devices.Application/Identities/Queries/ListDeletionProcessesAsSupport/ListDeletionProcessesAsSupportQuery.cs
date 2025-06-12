using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListDeletionProcessesAsSupport;

public class ListDeletionProcessesAsSupportQuery : IRequest<GetDeletionProcessesAsSupportResponse>
{
    public required string IdentityAddress { get; init; }
}
