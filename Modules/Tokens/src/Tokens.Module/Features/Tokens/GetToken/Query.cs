using Backbone.Modules.Tokens.Module.Features.Tokens.Shared;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.GetToken;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("GetTokenQuery")]
public class Query : IRequest<TokenDTO>
{
    public required string Id { get; init; }
    public byte[]? Password { get; init; }
}
