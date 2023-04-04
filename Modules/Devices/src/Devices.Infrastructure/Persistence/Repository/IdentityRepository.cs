using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
public class IdentityRepository : IIdentityRepository
{
    private readonly DbSet<Identity> _identityDbSet;
    private readonly IQueryable<Identity> _readonlyIdentityDbSet;

    public IdentityRepository(DevicesDbContext dbContext)
    {
        _identityDbSet = dbContext.Identities;
        _readonlyIdentityDbSet = dbContext.Identities.AsNoTracking();
    }

    public async Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter)
    {
        var paginationResult = await _readonlyIdentityDbSet.OrderAndPaginate(d => d.CreatedAt, paginationFilter);
        return paginationResult;
    }
}
