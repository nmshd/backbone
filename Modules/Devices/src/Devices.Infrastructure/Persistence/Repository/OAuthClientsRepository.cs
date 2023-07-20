using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
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

    public async Task<bool> Exists(string clientId, CancellationToken cancellationToken)
    {
        var client = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);
        return client != null;
    }

    public async Task Add(string clientId, string displayName, string clientSecret, CancellationToken cancellationToken)
    {
        await _applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
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
    }

    public async Task Delete(string clientId, CancellationToken cancellationToken)
    {
        var client = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        await _applicationManager.DeleteAsync(client, cancellationToken);
    }
}
