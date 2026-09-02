using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Challenges.Application.Challenges.Commands.ExecuteHousekeeping;

public class Handler : IRequestHandler<ExecuteHousekeepingCommand>
{
    private readonly IChallengesRepository _challengesRepository;
    private readonly HousekeepingTelemetry _telemetry;

    public Handler(IChallengesRepository challengesRepository, HousekeepingTelemetry telemetry)
    {
        _challengesRepository = challengesRepository;
        _telemetry = telemetry;
    }

    public async Task Handle(ExecuteHousekeepingCommand request, CancellationToken cancellationToken)
    {
        await DeleteChallenges(cancellationToken);
    }

    private async Task DeleteChallenges(CancellationToken cancellationToken)
    {
        await _telemetry.TrackDeletion("challenges", ct => _challengesRepository.Delete(Challenge.CanBeCleanedUp, ct), cancellationToken);
    }
}
