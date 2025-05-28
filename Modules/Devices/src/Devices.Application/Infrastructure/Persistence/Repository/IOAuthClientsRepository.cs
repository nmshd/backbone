using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IOAuthClientsRepository
{
    Task<IEnumerable<OAuthClient>> List(CancellationToken cancellationToken, bool track = false);
    Task<Dictionary<string, int>> CountIdentities(List<string> clients, CancellationToken cancellationToken);
    Task<OAuthClient?> Get(string clientId, CancellationToken cancellationToken, bool track = false);
    Task<bool> Exists(string clientId, CancellationToken cancellationToken);
    Task Add(OAuthClient client, string clientSecret, CancellationToken cancellationToken);
    Task Update(OAuthClient client, CancellationToken cancellationToken);
    Task ChangeClientSecret(OAuthClient client, string newSecret, CancellationToken cancellationToken);
    Task Delete(string clientId, CancellationToken cancellationToken);
}
