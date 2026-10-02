using Backbone.Modules.Devices.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsOwner;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetDeletionProcessAsOwnerQuery")]
public class Query : IRequest<IdentityDeletionProcessOverviewDTO>
{
    public required string Id { get; init; }
}
