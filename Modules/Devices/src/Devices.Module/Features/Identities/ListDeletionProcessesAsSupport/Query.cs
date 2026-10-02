using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsSupport;

public class ListDeletionProcessesAsSupportQuery : IRequest<GetDeletionProcessesAsSupportResponse>
{
    public required string IdentityAddress { get; init; }
}
