using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IIdentitiesRepository
{
    Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter);
    Task<Identity?> FindByAddress(IdentityAddress address, CancellationToken cancellationToken);
}
