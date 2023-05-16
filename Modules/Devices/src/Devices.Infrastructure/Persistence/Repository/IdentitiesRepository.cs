using Backbone.Modules.Devices.Infrastructure.Persistence.Database.QueryableExtensions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Microsoft.AspNetCore.Identity;
using Backbone.Modules.Devices.Application;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
public class IdentitiesRepository : IIdentitiesRepository
{
    private readonly DbSet<Identity> _identities;
    private readonly IQueryable<Identity> _readonlyIdentities;
    private readonly DevicesDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentitiesRepository(DevicesDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _identities = dbContext.Identities;
        _readonlyIdentities = dbContext.Identities.AsNoTracking();
        _dbContext = dbContext;
        _userManager = userManager;
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

    public async Task AddUserForIdentity(ApplicationUser user, string password)
    {
        var createUserResult = await _userManager.CreateAsync(user, password);
        if (!createUserResult.Succeeded)
            throw new OperationFailedException(ApplicationErrors.Devices.RegistrationFailed(createUserResult.Errors.First().Description));
    }

}
