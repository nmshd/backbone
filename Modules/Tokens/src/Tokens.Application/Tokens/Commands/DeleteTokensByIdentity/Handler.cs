using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensByIdentity;
public class Handler(ITokensRepository tokensRepository) : IRequestHandler<DeleteTokensByIdentityCommand>
{
    public async Task Handle(DeleteTokensByIdentityCommand request, CancellationToken cancellationToken)
    {
        await tokensRepository.DeleteAllOfOwner(request.IdentityAddress, cancellationToken);
    }
}
