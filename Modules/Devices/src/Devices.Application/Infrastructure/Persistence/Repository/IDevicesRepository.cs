using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IDevicesRepository
{
    Task<DbPaginationResult<Device>> FindAll(IdentityAddress identity, IEnumerable<DeviceId> ids, PaginationFilter paginationFilter);
    Task<Device> GetCurrentDevice(DeviceId deviceId, CancellationToken cancellationToken);
    Task<Device> GetDeviceByIdentityAndId(IdentityAddress identityAddress, DeviceId deviceId, CancellationToken cancellationToken);
    Task MarkAsDeleted(DeviceId id, byte[] deletionCertificate, DeviceId deletedByDevice, CancellationToken cancellationToken);
}
