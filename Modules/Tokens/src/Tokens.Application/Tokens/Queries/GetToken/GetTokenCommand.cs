using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class GetTokenCommand : IRequest<TokenDTO>
{
    public TokenId Id { get; set; }
}
