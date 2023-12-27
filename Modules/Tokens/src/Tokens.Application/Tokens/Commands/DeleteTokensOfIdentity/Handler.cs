using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteTokensOfIdentity;
public class Handler(ITokensRepository tokensRepository) : IRequestHandler<DeleteTokensOfIdentityCommand>
{
    public async Task Handle(DeleteTokensOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await tokensRepository.DeleteTokens(Token.WasCreatedBy(request.IdentityAddress), cancellationToken);
    }
}
