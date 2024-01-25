using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.ListIdentities;

public class FindAllStubRepository : IIdentitiesRepository
{
    private readonly DbPaginationResult<Identity> _identities;

    public FindAllStubRepository(DbPaginationResult<Identity> identities)
    {
        _identities = identities;
    }

    public Task<bool> Exists(IdentityAddress address, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountByClientId(string clientId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task AddUser(ApplicationUser user, string password)
    {
        throw new NotImplementedException();
    }

    public Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        return Task.FromResult(_identities);
    }

    public Task<DbPaginationResult<Device>> FindAllDevicesOfIdentity(IdentityAddress identity, IEnumerable<DeviceId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Device> GetDeviceById(DeviceId deviceId, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task Update(Device device, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(Identity identity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Identity?> FindByAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }
}
