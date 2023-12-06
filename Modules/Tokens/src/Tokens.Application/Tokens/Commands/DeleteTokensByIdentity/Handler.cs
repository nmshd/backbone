using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensByIdentity;
public class Handler(ITokensRepository tokensRepository) : IRequestHandler<DeleteTokensByIdentityCommand>
{
    private readonly ITokensRepository _tokensRepository = tokensRepository;

    public async Task Handle(DeleteTokensByIdentityCommand request, CancellationToken cancellationToken)
    {
        await _tokensRepository.DeleteAllOfOwner(request.IdentityAddress, cancellationToken);
    }
}
