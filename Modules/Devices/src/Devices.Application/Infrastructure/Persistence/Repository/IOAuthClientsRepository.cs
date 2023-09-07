using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Domain.OpenIddict;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IOAuthClientsRepository
{
    Task<IEnumerable<OAuthClient>> FindAll(CancellationToken cancellationToken);
    Task<CustomOpenIddictEntityFrameworkCoreApplication> Find(string clientId, CancellationToken cancellationToken);
    Task<bool> Exists(string clientId, CancellationToken cancellationToken);
    Task Add(string clientId, string displayName, string clientSecret, TierId tierId, CancellationToken cancellationToken);
    Task Update(CustomOpenIddictEntityFrameworkCoreApplication client, CancellationToken cancellationToken);
    Task ChangeClientSecret(CustomOpenIddictEntityFrameworkCoreApplication client, string clientSecret, CancellationToken cancellationToken);
    Task Delete(string clientId, CancellationToken cancellationToken);
}
