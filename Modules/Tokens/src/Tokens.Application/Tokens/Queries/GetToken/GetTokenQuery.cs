using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class GetTokenQuery : IRequest<TokenDTO>
{
    public required string Id { get; init; }
    public byte[]? Password { get; init; }
}
