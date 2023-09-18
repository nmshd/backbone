using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using OpenIddict.Core;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class OAuthClientsRepository : IOAuthClientsRepository
{
    private readonly OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> _applicationManager;
    private Dictionary<string, CustomOpenIddictEntityFrameworkCoreApplication> _trackedApplications;

    public OAuthClientsRepository(OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> applicationManager)
    {
        _applicationManager = applicationManager;
        _trackedApplications = new Dictionary<string, CustomOpenIddictEntityFrameworkCoreApplication>();
    }

    public async Task<IEnumerable<OAuthClient>> FindAll(CancellationToken cancellationToken, bool track = false)
    {
        var applications = await _applicationManager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken);

        var oAuthClients = new List<OAuthClient>();

        if (track)
        {
            foreach (var application in applications)
            {
                _trackedApplications[application.ClientId!] = application;
                oAuthClients.Add(new OAuthClient(application.ClientId!, application.DisplayName!, TierId.Create(application.DefaultTier).Value));
            }
        }
        else
        {
            oAuthClients.AddRange(applications.Select(application => new OAuthClient(application.ClientId!, application.DisplayName!, TierId.Create(application.DefaultTier).Value)));
        }

        return oAuthClients;
    }

    public async Task<OAuthClient> Find(string clientId, CancellationToken cancellationToken, bool track = false)
    {
        if (_trackedApplications.TryGetValue(clientId, out var trackedApplication))
        {
            return new OAuthClient(trackedApplication.ClientId!, trackedApplication.DisplayName!, TierId.Create(trackedApplication.DefaultTier).Value);
        }

        var application = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);

        if (application == null)
            return null;
        
        if(track)
            _trackedApplications[clientId] = application;

        return new OAuthClient(application.ClientId!, application.DisplayName!, TierId.Create(application.DefaultTier).Value);
    }

    public async Task<bool> Exists(string clientId, CancellationToken cancellationToken)
    {
        var client = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);
        return client != null;
    }

    public async Task Add(string clientId, string displayName, string newSecret, TierId tierId, CancellationToken cancellationToken)
    {
        var application = new CustomOpenIddictEntityFrameworkCoreApplication()
        {
            ClientId = clientId,
            DisplayName = displayName,
            DefaultTier = tierId,
            Permissions = GetPermissions()
        };

        await _applicationManager.CreateAsync(application, newSecret, cancellationToken);
    }

    private static string GetPermissions()
    {
        var permissions = new List<string>()
        {
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.Password
        };

        using var stream = new MemoryStream();
        var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        });

        writer.WriteStartArray();

        foreach (var permission in permissions)
        {
            writer.WriteStringValue(permission);
        }

        writer.WriteEndArray();
        writer.Flush();

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public async Task Update(OAuthClient client, CancellationToken cancellationToken)
    {
        if (!_trackedApplications.TryGetValue(client.ClientId, out var application))
        {
            application = await _applicationManager.FindByClientIdAsync(client.ClientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        }
        application.DefaultTier = client.DefaultTier;
        await _applicationManager.UpdateAsync(application, cancellationToken);
    }

    public async Task Delete(string clientId, CancellationToken cancellationToken)
    {
        if (!_trackedApplications.TryGetValue(clientId, out var application))
        {
            application = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        }
        await _applicationManager.DeleteAsync(application, cancellationToken);
        _trackedApplications.Remove(clientId);
    }

    public async Task ChangeClientSecret(OAuthClient client, string clientSecret, CancellationToken cancellationToken)
    {
        if (!_trackedApplications.TryGetValue(client.ClientId, out var application))
        {
            application = await _applicationManager.FindByClientIdAsync(client.ClientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        }
        await _applicationManager.UpdateAsync(application, clientSecret, cancellationToken);
    }
}
