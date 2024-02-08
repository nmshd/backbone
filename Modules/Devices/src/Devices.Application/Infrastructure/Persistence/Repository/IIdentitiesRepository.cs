using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IIdentitiesRepository
{
    #region Identities
    Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task Update(Identity identity, CancellationToken cancellationToken);
    Task<Identity?> FindByAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false);
    Task<bool> Exists(IdentityAddress address, CancellationToken cancellationToken);
    Task<IEnumerable<Identity>> Find(Expression<Func<Identity, bool>> filter, CancellationToken cancellationToken, bool track = false);
    Task<IEnumerable<Identity>> FindAllWithDeletionProcessInStatus(DeletionProcessStatus status, CancellationToken cancellationToken, bool track = false);
    Task Delete(Expression<Func<Identity, bool>> filter, CancellationToken cancellationToken);
    Task<int> CountByClientId(string clientId, CancellationToken cancellationToken);
    #endregion

    #region Users
    Task AddUser(ApplicationUser user, string password);
    #endregion

    #region Devices
    Task<DbPaginationResult<Device>> FindAllDevicesOfIdentity(IdentityAddress identity, IEnumerable<DeviceId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task<Device> GetDeviceById(DeviceId deviceId, CancellationToken cancellationToken, bool track = false);
    Task Update(Device device, CancellationToken cancellationToken);
    #endregion
}
