using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.ListIdentities;

public class FindAllStubRepository : IIdentitiesRepository
{
    private readonly IEnumerable<Identity> _identities;

    public FindAllStubRepository(IEnumerable<Identity> identities)
    {
        _identities = identities;
    }

    public Task<bool> Exists(IdentityAddress address, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<IEnumerable<Identity>> FindAllWithDeletionProcessInStatus(DeletionProcessStatus status, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotSupportedException();
    }

    public Task<int> CountByClientId(string clientId, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task AddUser(ApplicationUser user, string password)
    {
        throw new NotSupportedException();
    }

    public Task<DbPaginationResult<Device>> FindAllDevicesOfIdentity(IdentityAddress identity, IEnumerable<DeviceId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<Device?> GetDeviceById(DeviceId deviceId, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotSupportedException();
    }

    public Task Update(Device device, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<IEnumerable<IdentityDeletionProcessAuditLogEntry>> GetIdentityDeletionProcessAuditLogsByAddress(byte[] identityAddressHash, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task AddDeletionProcessAuditLogEntry(IdentityDeletionProcessAuditLogEntry auditLogEntry)
    {
        throw new NotSupportedException();
    }

    public Task Update(Identity identity, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<Identity?> FindByAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false)
    {
        throw new NotSupportedException();
    }

    public Task<IEnumerable<Identity>> Find(Expression<Func<Identity, bool>> filter, CancellationToken cancellationToken, bool track = false)
    {
        return Task.FromResult(_identities);
    }

    public Task Delete(Expression<Func<Identity, bool>> filter, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}
