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

    public async Task AddClients(string clientId, string displayName, string clientSecret, CancellationToken cancellationToken)
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
