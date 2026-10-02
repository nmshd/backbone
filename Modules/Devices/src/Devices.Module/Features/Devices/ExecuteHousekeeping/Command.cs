using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Devices.ExecuteHousekeeping;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ExecuteHousekeepingCommand")]
public class Command : IRequest;
