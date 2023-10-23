using Backbone.Tokens.Application.Tokens.DTOs;
using Backbone.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Tokens.Application.Tokens.Queries.GetToken;

public class GetTokenQuery : IRequest<TokenDTO>
{
    public TokenId Id { get; set; }
}
