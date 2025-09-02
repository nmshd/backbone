using Backbone.BuildingBlocks.Application.Abstractions.Housekeeping;
using Backbone.Modules.Challenges.Application.Challenges.Commands.DeleteExpiredChallenges;
using Backbone.Modules.Challenges.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Challenges.Domain.Entities;

namespace Backbone.Modules.Challenges.Application;

public class Housekeeper : IHousekeeper
{
    private readonly IChallengesRepository _challengesRepository;

    public Housekeeper(IChallengesRepository challengesRepository)
    {
        _challengesRepository = challengesRepository;
    }

    public async Task<HousekeepingResponse> Execute(CancellationToken cancellationToken)
    {
        var responseItem = await DeleteChallenges(cancellationToken);
        return new HousekeepingResponse { Items = [responseItem] };
    }

    private async Task<HousekeepingResponseItem> DeleteChallenges(CancellationToken cancellationToken)
    {
        var deletedChallengesCount = await _challengesRepository.Delete(Challenge.CanBeDeleted, cancellationToken);

        return new HousekeepingResponseItem { EntityType = typeof(Challenge), NumberOfDeletedEntities = deletedChallengesCount };
    }
}
