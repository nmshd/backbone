using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IIdentitiesRepository
{
    #region Identities
    Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter);
#nullable enable
    Task<Identity?> FindByAddress(IdentityAddress address, CancellationToken cancellationToken);
#nullable disable
    #endregion

    #region Users
    Task AddUser(ApplicationUser user, string password);
    #endregion

    #region Devices
    Task<DbPaginationResult<Device>> FindAllDevicesOfIdentity(IdentityAddress identity, IEnumerable<DeviceId> ids, PaginationFilter paginationFilter);
    Task<Device> GetDeviceById(DeviceId deviceId, CancellationToken cancellationToken, bool track = false);
    Task Update(Device device, CancellationToken cancellationToken);
    #endregion
}
