using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ExecuteHousekeeping;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ExecuteHousekeepingCommand")]
public class Command : IRequest;
