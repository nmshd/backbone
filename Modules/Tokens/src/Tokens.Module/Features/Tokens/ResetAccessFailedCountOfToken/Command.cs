using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ResetAccessFailedCountOfToken;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ResetAccessFailedCountOfTokenCommand")]
public class Command : IRequest
{
    public required string TokenId { get; init; }
}
