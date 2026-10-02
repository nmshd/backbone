using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokensForIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("AnonymizeTokensForIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
