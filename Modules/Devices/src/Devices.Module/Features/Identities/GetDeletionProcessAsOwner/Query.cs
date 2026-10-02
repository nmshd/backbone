using Backbone.Modules.Devices.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsOwner;

public class GetDeletionProcessAsOwnerQuery : IRequest<IdentityDeletionProcessOverviewDTO>
{
    public required string Id { get; init; }
}
