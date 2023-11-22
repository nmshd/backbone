using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Identities.Commands.DeleteIdentity;

public class Handler : IRequestHandler<DeleteIdentityCommand>
{
    private readonly IChallengesRepository _challengesRepository;

    public Handler(IChallengesRepository challengesRepository)
    {
        _challengesRepository = challengesRepository;
    }

    public async Task Handle(DeleteIdentityCommand request, CancellationToken cancellationToken)
    {
        await _challengesRepository.DeleteChallengesByIdentityAddress(request.IdentityAddress, cancellationToken);
    }
}
