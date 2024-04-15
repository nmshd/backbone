using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteChallengesOfIdentity;

public class Handler : IRequestHandler<DeleteChallengesOfIdentityCommand>
{
    private readonly IChallengesRepository _challengesRepository;

    public Handler(IChallengesRepository challengesRepository)
    {
        _challengesRepository = challengesRepository;
    }

    public async Task Handle(DeleteChallengesOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await _challengesRepository.Delete(Challenge.WasCreatedBy(request.IdentityAddress), cancellationToken);
    }
}
