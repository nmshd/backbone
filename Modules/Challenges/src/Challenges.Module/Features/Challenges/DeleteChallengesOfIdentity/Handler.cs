using Backbone.Modules.Challenges.Abstractions;
using Backbone.Modules.Challenges.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.DeleteChallengesOfIdentity;

public class Handler : IRequestHandler<Command>
{
    private readonly IChallengesRepository _challengesRepository;

    public Handler(IChallengesRepository challengesRepository)
    {
        _challengesRepository = challengesRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await _challengesRepository.Delete(Challenge.WasCreatedBy(request.IdentityAddress), cancellationToken);
    }
}
