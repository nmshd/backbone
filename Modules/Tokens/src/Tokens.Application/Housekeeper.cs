using Backbone.BuildingBlocks.Application.Abstractions.Housekeeping;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application;

public class Housekeeper : IHousekeeper
{
    private readonly ITokensRepository _tokensRepository;

    public Housekeeper(ITokensRepository tokensRepository)
    {
        _tokensRepository = tokensRepository;
    }

    public async Task<HousekeepingResponse> Execute(CancellationToken cancellationToken)
    {
        var responseItem = await DeleteTokens(cancellationToken);
        return new HousekeepingResponse { Items = [responseItem] };
    }

    private async Task<HousekeepingResponseItem> DeleteTokens(CancellationToken cancellationToken)
    {
        var deletedTokensCount = await _tokensRepository.Delete(Token.CanBeCleanedUp, cancellationToken);

        return new HousekeepingResponseItem { EntityType = typeof(Token), NumberOfDeletedEntities = deletedTokensCount };
    }
}
