using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class GetTokenQuery : IRequest<TokenDTO>
{
    public required TokenId Id { get; set; }
}
