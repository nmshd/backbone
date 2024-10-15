using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class Handler : IRequestHandler<GetTokenQuery, TokenDTO>
{
    private readonly ITokensRepository _tokensRepository;
    private readonly IUserContext _userContext;

    public Handler(ITokensRepository tokensRepository, IUserContext userContext)
    {
        _tokensRepository = tokensRepository;
        _userContext = userContext;
    }

    public async Task<TokenDTO> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        var token = await GetToken(request.Id, request.Password, cancellationToken);
        return new TokenDTO(token);
    }

    private async Task<Token> GetToken(string tokenId, byte[]? password, CancellationToken cancellationToken)
    {
        var token = await _tokensRepository.Find(TokenId.Parse(tokenId), _userContext.GetAddressOrNull(), cancellationToken, true) ??
                    throw new NotFoundException(nameof(Token));

        if (!token.CanBeCollectedUsingPassword(_userContext.GetAddressOrNull(), password))
            throw new NotFoundException(nameof(Token));

        return token;
    }
}
