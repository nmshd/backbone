using Backbone.Modules.Devices.Domain.Entities;
using OpenIddict.EntityFrameworkCore.Models;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IOAuthClientsRepository
{
    Task<IEnumerable<OAuthClient>> FindAll(CancellationToken cancellationToken);
    Task<OpenIddictEntityFrameworkCoreApplication> Find(string clientId, CancellationToken cancellationToken);
    Task<bool> Exists(string clientId, CancellationToken cancellationToken);
    Task Add(string clientId, string displayName, string clientSecret, CancellationToken cancellationToken);
    Task Update(OpenIddictEntityFrameworkCoreApplication client, CancellationToken cancellationToken);
    Task Delete(string clientId, CancellationToken cancellationToken);
}
