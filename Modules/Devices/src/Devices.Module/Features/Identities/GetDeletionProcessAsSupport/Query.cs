using Backbone.Modules.Devices.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsSupport;

public class Query : IRequest<IdentityDeletionProcessDetailsDTO>
{
    public required string IdentityAddress { get; init; }
    public required string DeletionProcessId { get; init; }
}
