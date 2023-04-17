using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface ITiersRepository
{
    Task AddAsync(Tier tier, CancellationToken cancellationToken);

    Task<DbPaginationResult<Tier>> FindAll(PaginationFilter paginationFilter);
}
