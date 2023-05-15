using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.ListIdentities;
public class FindAllStubRepository : IIdentitiesRepository
{
    private readonly DbPaginationResult<Identity> _identities;

    public FindAllStubRepository(DbPaginationResult<Identity> identities)
    {
        _identities = identities;
    }

    public Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter)
    {
        return Task.FromResult(_identities);
    }

    public Task<Identity> FindByAddress(IdentityAddress address, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
