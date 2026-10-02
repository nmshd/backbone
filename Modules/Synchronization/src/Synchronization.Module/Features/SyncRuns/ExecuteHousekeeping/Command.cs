using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.ExecuteHousekeeping;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ExecuteHousekeepingCommand")]
public class Command : IRequest;
