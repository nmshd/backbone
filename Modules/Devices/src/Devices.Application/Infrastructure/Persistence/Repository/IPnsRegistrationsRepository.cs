using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IPnsRegistrationsRepository
{
    Task Add(PnsRegistration registration, CancellationToken cancellationToken);
    Task Update(PnsRegistration registration, CancellationToken cancellationToken);
    Task<PnsRegistration?> FindByDeviceId(DeviceId deviceId, CancellationToken cancellationToken, bool track = false);
    Task<PnsRegistration[]> FindDistinctByDeviceIds(DeviceId[] deviceIds, CancellationToken cancellationToken, bool track = false);
    Task<int> Delete(List<DeviceId> deviceIds, CancellationToken cancellationToken);
    Task Delete(Expression<Func<PnsRegistration, bool>> filter, CancellationToken cancellationToken);
}
