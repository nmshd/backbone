using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;
public class Handler(ITokensRepository tokensRepository) : IRequestHandler<DeleteTokensOfIdentityCommand>
{
    public async Task Handle(DeleteTokensOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await tokensRepository.DeleteAllOfOwner(request.IdentityAddress, cancellationToken);
    }
}
