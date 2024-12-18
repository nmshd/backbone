using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

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

        var identity = _userContext.GetAddressOrNull();
        if (identity == null)
        {
            await EnsureTokenCanBeCollected(token, null, password, cancellationToken);
        }
        else if (identity != token.CreatedBy)
        {
            if (!token.HasAllocationForIdentity(identity))
            {
                await EnsureTokenCanBeCollected(token, identity, password, cancellationToken);

                token.AddAllocationFor(identity, _userContext.GetDeviceId());
                await _tokensRepository.Update(token, cancellationToken);
            }
        }

        return token;
    }

    private async Task EnsureTokenCanBeCollected(Token token, IdentityAddress? address, byte[]? password, CancellationToken cancellationToken)
    {
        if (token.IsLocked) throw new ApplicationException(ApplicationErrors.TokenIsLocked());

        if (!token.CanBeCollectedUsingPassword(address, password))
        {
            token.IncrementAccessFailedCount();
            await _tokensRepository.Update(token, cancellationToken);

            if (token.IsLocked) throw new ApplicationException(ApplicationErrors.TokenIsLocked());
            throw new NotFoundException(nameof(Token));
        }
    }
}
