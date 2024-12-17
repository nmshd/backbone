using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IIdentitiesRepository
{
    #region Identities

    Task Update(Identity identity, CancellationToken cancellationToken);
    Task<Identity?> FindByAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false);
    Task<bool> Exists(IdentityAddress address, CancellationToken cancellationToken);
    Task<IEnumerable<Identity>> FindAllWithDeletionProcessInStatus(DeletionProcessStatus status, CancellationToken cancellationToken, bool track = false);
    Task<int> CountByClientId(string clientId, CancellationToken cancellationToken);
    Task<IEnumerable<Identity>> Find(Expression<Func<Identity, bool>> filter, CancellationToken cancellationToken, bool track = false);
    Task<Identity?> FindFirst(Expression<Func<Identity, bool>> filter, CancellationToken cancellationToken, bool track = false);
    Task Delete(Expression<Func<Identity, bool>> filter, CancellationToken cancellationToken);

    Task Add(Identity identity, string password);
    Task UpdateWithNewDevice(Identity identity, string password);

    #endregion

    #region Devices

    Task<DbPaginationResult<Device>> FindAllDevicesOfIdentity(IdentityAddress identity, IEnumerable<DeviceId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task<Device?> GetDeviceById(DeviceId deviceId, CancellationToken cancellationToken, bool track = false);
    Task<IEnumerable<Device>> GetDevicesByIds(IEnumerable<DeviceId> deviceIds, CancellationToken cancellationToken, bool track = false);
    Task Update(Device device, CancellationToken cancellationToken);
    Task<T[]> FindDevices<T>(Expression<Func<Device, bool>> filter, Expression<Func<Device, T>> selector, CancellationToken cancellationToken, bool track = false);
    Task<bool> HasBackupDevice(IdentityAddress identity, CancellationToken cancellationToken);
    Task DeleteDevice(Device device, CancellationToken cancellationToken);

    #endregion

    #region Deletion Process Audit Logs

    Task<IEnumerable<IdentityDeletionProcessAuditLogEntry>> GetIdentityDeletionProcessAuditLogs(Expression<Func<IdentityDeletionProcessAuditLogEntry, bool>> filter,
        CancellationToken cancellationToken, bool track = false);

    Task AddDeletionProcessAuditLogEntry(IdentityDeletionProcessAuditLogEntry auditLogEntry);

    Task Update(IEnumerable<IdentityDeletionProcessAuditLogEntry> auditLogEntries, CancellationToken cancellationToken);

    #endregion
}
