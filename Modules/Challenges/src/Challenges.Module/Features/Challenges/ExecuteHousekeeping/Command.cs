using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.ExecuteHousekeeping;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ExecuteHousekeepingCommand")]
public class Command : IRequest;
