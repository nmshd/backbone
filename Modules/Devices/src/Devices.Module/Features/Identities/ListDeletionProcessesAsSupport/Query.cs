using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsSupport;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListDeletionProcessesAsSupportQuery")]
public class Query : IRequest<Response>
{
    public required string IdentityAddress { get; init; }
}
