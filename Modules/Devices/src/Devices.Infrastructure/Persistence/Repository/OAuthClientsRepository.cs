using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class OAuthClientsRepository : IOAuthClientsRepository
{
    private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> _applicationManager;

    public OAuthClientsRepository(OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager)
    {
        _applicationManager = applicationManager;
    }

    public async Task<IEnumerable<OAuthClient>> FindAll(CancellationToken cancellationToken)
    {
        var clients = await _applicationManager.ListAsync(applications => applications.Select(c => new OAuthClient(c.ClientId, c.DisplayName)), cancellationToken).ToListAsync(cancellationToken);
        return clients;
    }

    public async Task<OAuthClient> Find(string clientId, CancellationToken cancellationToken)
    {
        var client = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);
        return client != null ? new OAuthClient(client.ClientId, client.DisplayName) : null;
    }

    public async Task Add(string clientId, string displayName, string clientSecret, CancellationToken cancellationToken)
    {
        var managerResult = await _applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            DisplayName = displayName,
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.Password
            }
        }, cancellationToken);

        if (managerResult == null)
            throw new Exception($"Failed to create the client: '{displayName}'");
    }
}
