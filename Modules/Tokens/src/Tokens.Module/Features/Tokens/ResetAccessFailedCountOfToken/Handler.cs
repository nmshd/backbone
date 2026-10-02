using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Tokens.Abstractions;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ResetAccessFailedCountOfToken;

public class Handler : IRequestHandler<Command>
{
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var token = await _tokensRepository.GetWithoutContent(TokenId.Parse(request.TokenId), cancellationToken) ?? throw new NotFoundException(nameof(Token));
        token.ResetAccessFailedCount();
        await _tokensRepository.Update(token, cancellationToken);
    }
}
