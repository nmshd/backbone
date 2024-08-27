using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class Handler : IRequestHandler<GetTokenQuery, TokenDTO>
{
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task<TokenDTO> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        var token = await _tokensRepository.Find(TokenId.Parse(request.Id));
        return new TokenDTO(token);
    }
}
