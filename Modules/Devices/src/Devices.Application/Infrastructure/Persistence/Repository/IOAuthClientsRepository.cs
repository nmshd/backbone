using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IOAuthClientsRepository
{
    Task<IEnumerable<OAuthClient>> FindAll(CancellationToken cancellationToken);
}
