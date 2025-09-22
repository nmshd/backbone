using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;
public class Handler : IRequestHandler<DeleteTokensOfIdentityCommand>
{
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task Handle(DeleteTokensOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await _tokensRepository.Delete(Token.WasCreatedBy(request.IdentityAddress), cancellationToken);
    }
}
