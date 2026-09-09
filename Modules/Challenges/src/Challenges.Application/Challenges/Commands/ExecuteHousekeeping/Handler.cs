using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IChallengesRepository _challengesRepository;

    public Handler(IChallengesRepository challengesRepository)
    {
        _challengesRepository = challengesRepository;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteChallenges(cancellationToken);
    }

    private async Task DeleteChallenges(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("challenges", ct => _challengesRepository.Delete(Challenge.CanBeCleanedUp, ct), cancellationToken);
    }
}
