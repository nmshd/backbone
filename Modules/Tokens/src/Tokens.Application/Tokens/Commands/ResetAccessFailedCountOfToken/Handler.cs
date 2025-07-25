using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.ResetAccessFailedCountOfToken;

public class Handler : IRequestHandler<ResetAccessFailedCountOfTokenCommand>
{
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task Handle(ResetAccessFailedCountOfTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _tokensRepository.GetWithoutContent(TokenId.Parse(request.TokenId), cancellationToken) ?? throw new NotFoundException(nameof(Token));
        token.ResetAccessFailedCount();
        await _tokensRepository.Update(token, cancellationToken);
    }
}
