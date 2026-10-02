using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.CancelDeletionProcess;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CancelDeletionProcessCommand")]
public class Command : IRequest<Response>
{
    public required string DeletionProcessId { get; init; }
}
