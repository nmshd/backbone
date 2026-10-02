using Backbone.Modules.Tokens.Abstractions;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.DeleteTokensOfIdentity;

public class Handler : IRequestHandler<Command>
{
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await _tokensRepository.Delete(Token.WasCreatedBy(request.IdentityAddress), cancellationToken);
    }
}
