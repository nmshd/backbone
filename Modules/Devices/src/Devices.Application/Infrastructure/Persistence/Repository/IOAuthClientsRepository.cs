using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IOAuthClientsRepository
{
    Task<IEnumerable<OAuthClient>> FindAll(CancellationToken cancellationToken);
    Task<OAuthClient> Find(string clientId, CancellationToken cancellationToken);
    Task Add(string clientId, string displayName, string clientSecret, CancellationToken cancellationToken);
}
