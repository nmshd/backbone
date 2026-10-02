using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.DeleteTokensOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeleteTokensOfIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
