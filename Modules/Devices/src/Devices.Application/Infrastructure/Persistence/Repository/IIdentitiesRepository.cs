using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IIdentitiesRepository
{
    Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter);
#nullable enable
    Task<Identity?> FindByAddress(IdentityAddress address, CancellationToken cancellationToken);
#nullable disable
    Task AddUserForIdentity(ApplicationUser user, string password);
}
