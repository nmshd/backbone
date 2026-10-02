using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.DeleteToken;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteTokenCommand")]
public class Command : IRequest
{
    public required string Id { get; init; }
}
