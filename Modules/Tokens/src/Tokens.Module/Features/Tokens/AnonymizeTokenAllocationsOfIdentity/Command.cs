using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.AnonymizeTokenAllocationsOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("AnonymizeTokenAllocationsOfIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
