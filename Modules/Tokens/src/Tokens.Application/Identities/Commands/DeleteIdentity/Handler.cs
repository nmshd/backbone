using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Identities.Commands.DeleteIdentity;
public class Handler : IRequestHandler<DeleteIdentityCommand>
{
    private readonly ITokensRepository _tokensRepository;

    public Handler(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }
    public async Task Handle(DeleteIdentityCommand request, CancellationToken cancellationToken)
    {
        await _tokensRepository.DeleteAllOfOwner(request.IdentityAddress, cancellationToken);
    }
}
