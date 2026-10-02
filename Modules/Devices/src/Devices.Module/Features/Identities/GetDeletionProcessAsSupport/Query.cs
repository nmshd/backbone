using Backbone.Modules.Devices.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsSupport;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetDeletionProcessAsSupportQuery")]
public class Query : IRequest<IdentityDeletionProcessDetailsDTO>
{
    public required string IdentityAddress { get; init; }
    public required string DeletionProcessId { get; init; }
}
