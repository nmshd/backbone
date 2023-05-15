using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IDevicesRepository
{
    Task<DbPaginationResult<Device>> FindAll(IdentityAddress identity, IEnumerable<DeviceId> ids, PaginationFilter paginationFilter);
    Task<Device> GetDeviceById(DeviceId deviceId, CancellationToken cancellationToken, bool track = false);
    Task MarkAsDeleted(DeviceId id, byte[] deletionCertificate, DeviceId deletedByDevice, CancellationToken cancellationToken);
    Task Update(Device device, CancellationToken cancellationToken);
}
