using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.TriggerRipeDeletionProcesses;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("TriggerRipeDeletionProcessesCommand")]
public class Command : IRequest<Response>;
