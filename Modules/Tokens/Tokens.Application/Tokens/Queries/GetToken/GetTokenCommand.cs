using MediatR;
using Tokens.Application.Tokens.DTOs;
using Tokens.Domain.Entities;

namespace Tokens.Application.Tokens.Queries.GetToken;

public class GetTokenCommand : IRequest<TokenDTO>
{
    public TokenId Id { get; set; }
}
