using Backbone.Modules.Devices.Application.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsOwner;

public class GetDeletionProcessAsOwnerQuery : IRequest<IdentityDeletionProcessOverviewDTO>
{
    public required string Id { get; init; }
}
