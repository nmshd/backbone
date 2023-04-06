using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
public class IdentitiesRepository : IIdentitiesRepository
{
    private readonly DbSet<Identity> _identities;
    private readonly IQueryable<Identity> _readonlyIdentities;

    public IdentitiesRepository(DevicesDbContext dbContext)
    {
        _identities = dbContext.Identities;
        _readonlyIdentities = dbContext.Identities.AsNoTracking();
    }

    public async Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter)
    {
        var paginationResult = await _readonlyIdentities
            .Include(i => i.Devices)
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter);
        return paginationResult;
    }
}
