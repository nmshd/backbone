﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IIdentitiesRepository
{
    #region Identities
    Task<DbPaginationResult<Identity>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task Update(Identity identity, CancellationToken cancellationToken);
#nullable enable
    Task<Identity?> FindByAddress(IdentityAddress address, CancellationToken cancellationToken, bool track = false);
#nullable disable
    Task<bool> Exists(IdentityAddress address, CancellationToken cancellationToken);
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
