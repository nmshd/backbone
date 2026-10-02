using Backbone.BuildingBlocks.Application.Housekeeping;
using Backbone.Modules.Challenges.Abstractions;
using Backbone.Modules.Challenges.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.ExecuteHousekeeping;

public class Handler : IRequestHandler<Command>
{
    private readonly IChallengesRepository _challengesRepository;

    public Handler(IChallengesRepository challengesRepository)
    {
        _challengesRepository = challengesRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await DeleteChallenges(cancellationToken);
    }

    private async Task DeleteChallenges(CancellationToken cancellationToken)
    {
        await HousekeepingTelemetry.TrackItemDeletion("challenges", ct => _challengesRepository.Delete(Challenge.CanBeCleanedUp, ct), cancellationToken);
    }
}
