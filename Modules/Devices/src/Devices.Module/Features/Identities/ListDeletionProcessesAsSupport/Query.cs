using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsSupport;

public class Query : IRequest<Response>
{
    public required string IdentityAddress { get; init; }
}
