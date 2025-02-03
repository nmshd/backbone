using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
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
        var token = await _tokensRepository.Find(TokenId.Parse(tokenId), cancellationToken, track: true) ??
                    throw new NotFoundException(nameof(Token));

        var activeIdentity = _userContext.GetAddressOrNull();
        var activeDevice = _userContext.GetDeviceIdOrNull();
        var result = token.TryToAccess(activeIdentity, activeDevice, password);

        switch (result)
        {
            case TokenAccessResult.AllocationAdded:
                await _tokensRepository.Update(token, cancellationToken);
                return token;

            case TokenAccessResult.WrongPassword:
                await _tokensRepository.Update(token, cancellationToken);
                throw new NotFoundException(nameof(Token));

            case TokenAccessResult.Locked:
                await _tokensRepository.Update(token, cancellationToken);
                throw new ApplicationException(ApplicationErrors.TokenIsLocked());

            case TokenAccessResult.ForIdentityDoesNotMatch or TokenAccessResult.Expired:
                throw new NotFoundException(nameof(Token));

            case TokenAccessResult.Ok:
                return token;

            default:
                throw new Exception("Unexpected token access result.");
        }
    }
}
