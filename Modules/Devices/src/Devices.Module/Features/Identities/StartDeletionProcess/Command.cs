using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.StartDeletionProcess;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("StartDeletionProcessCommand")]
public class Command : IRequest<Response>
{
    public double? LengthOfGracePeriodInDays { get; init; }
}
