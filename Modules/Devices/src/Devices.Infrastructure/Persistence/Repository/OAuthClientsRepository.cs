using System.Linq;
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
    private List<CustomOpenIddictEntityFrameworkCoreApplication> _cachedApplications;

    public OAuthClientsRepository(OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> applicationManager)
    {
        _applicationManager = applicationManager;
        _cachedApplications = new List<CustomOpenIddictEntityFrameworkCoreApplication>();
    }

    public async Task<IEnumerable<OAuthClient>> FindAll(CancellationToken cancellationToken)
    {
        var applications = await _applicationManager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken);

        _cachedApplications = applications;

        return applications.Select(client => new OAuthClient(client.ClientId!, client.DisplayName!, TierId.Create(client.DefaultTier).Value)).ToList();
    }

    public async Task<OAuthClient> Find(string clientId, CancellationToken cancellationToken, bool track = false)
    {
        var application = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));

        var cachedApplicationIndex = _cachedApplications.FindIndex(cachedApplication => cachedApplication.ClientId == clientId);
        if (cachedApplicationIndex != -1)
        {
            _cachedApplications[cachedApplicationIndex] = application;
        }
        else
        {
            _cachedApplications.Add(application);
        }

        return new OAuthClient(application.ClientId!, application.DisplayName!, TierId.Create(application.DefaultTier).Value);
    }

    public async Task<bool> Exists(string clientId, CancellationToken cancellationToken)
    {
        var client = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);
        return client != null;
    }

    public async Task Add(string clientId, string displayName, string clientSecret, TierId tierId, CancellationToken cancellationToken)
    {
        var application = new CustomOpenIddictEntityFrameworkCoreApplication()
        {
            ClientId = clientId,
            DisplayName = displayName,
            DefaultTier = tierId,
            Permissions = GetPermissions()
        };

        await _applicationManager.CreateAsync(application, clientSecret, cancellationToken);
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
        var cachedApplication = _cachedApplications.FirstOrDefault(ca => ca.ClientId == client.ClientId);
        if (cachedApplication != null)
        {
            cachedApplication.DefaultTier = client.DefaultTier;
            await _applicationManager.UpdateAsync(cachedApplication, cancellationToken);
            return;
        }

        var application = await _applicationManager.FindByIdAsync(client.ClientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        application.DefaultTier = client.DefaultTier;
        await _applicationManager.UpdateAsync(application, cancellationToken);
    }

    public async Task Delete(string clientId, CancellationToken cancellationToken)
    {
        var cachedApplication = _cachedApplications.FirstOrDefault(ca => ca.ClientId == clientId);
        if (cachedApplication != null)
        {
            await _applicationManager.DeleteAsync(cachedApplication, cancellationToken);
            _cachedApplications.Remove(cachedApplication);
            return;
        }

        var client = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        await _applicationManager.DeleteAsync(client, cancellationToken);
    }

    public async Task ChangeClientSecret(OAuthClient client, string clientSecret, CancellationToken cancellationToken)
    {
        var cachedApplication = _cachedApplications.FirstOrDefault(ca => ca.ClientId == client.ClientId);
        if (cachedApplication != null)
        {
            await _applicationManager.UpdateAsync(cachedApplication, clientSecret, cancellationToken);
            _cachedApplications.Remove(cachedApplication);
            return;
        }

        var application = await _applicationManager.FindByIdAsync(client.ClientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        await _applicationManager.UpdateAsync(application, clientSecret, cancellationToken);
    }
}
