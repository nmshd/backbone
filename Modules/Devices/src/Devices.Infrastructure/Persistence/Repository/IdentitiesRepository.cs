using Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
public class IdentitiesRepository : IIdentitiesRepository
{
    private readonly DbSet<Identity> _identities;
    private readonly IQueryable<Identity> _readonlyIdentities;
    private readonly DevicesDbContext _dbContext;

    public IdentitiesRepository(DevicesDbContext dbContext)
    {
        _identities = dbContext.Identities;
        _readonlyIdentities = dbContext.Identities.AsNoTracking();
        _dbContext = dbContext;
    }

    public async Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter)
    {
        var paginationResult = await _readonlyIdentities
            .IncludeAll(_dbContext)
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter);
        return paginationResult;
    }

    public async Task<Identity> FindByAddress(IdentityAddress address, CancellationToken cancellationToken)
    {
        return await _readonlyIdentities
            .IncludeAll(_dbContext)
            .FirstWithAddressOrDefault(address, cancellationToken);
    }
}
