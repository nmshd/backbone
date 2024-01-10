using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetIdentity;

public class FindByAddressStubRepository : IIdentitiesRepository
{
    private readonly Identity _identity;

    public FindByAddressStubRepository(Identity identity)
    {
        _identity = identity;
    }

    public Task<bool> Exists(IdentityAddress address, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Identity>> FindAllWithApprovedDeletionProcess(CancellationToken cancellationToken, bool track = false)
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
        throw new NotImplementedException();
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

    public Task<Identity> FindByAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false)
    {
        return Task.FromResult(_identity);
    }

    public Task<IEnumerable<Identity>> FindAllWithPastDeletionGracePeriod(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Identity>> FindAllToBeDeletedWithPastDeletionGracePeriod(CancellationToken cancellationToken, bool track = false)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Identity identity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDevices(Expression<Func<Device, bool>> expression, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
