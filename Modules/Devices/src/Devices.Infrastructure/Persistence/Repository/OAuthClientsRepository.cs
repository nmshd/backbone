using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Domain.OpenIddict;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using OpenIddict.Core;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class OAuthClientsRepository : IOAuthClientsRepository
{
    private readonly OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> _applicationManager;

    public OAuthClientsRepository(OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> applicationManager)
    {
        _applicationManager = applicationManager;
    }

    public async Task<IEnumerable<OAuthClient>> FindAll(CancellationToken cancellationToken)
    {
        var clients = await _applicationManager.ListAsync(applications => applications.Select(c => new OAuthClient(c.ClientId, c.DisplayName, c.TierId)), cancellationToken).ToListAsync(cancellationToken);
        return clients;
    }

    public async Task<CustomOpenIddictEntityFrameworkCoreApplication> Find(string clientId, CancellationToken cancellationToken)
    {
        return await _applicationManager.FindByClientIdAsync(clientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
    }

    public async Task<bool> Exists(string clientId, CancellationToken cancellationToken)
    {
        var client = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);
        return client != null;
    }

    public async Task Add(string clientId, string displayName, string clientSecret, string tierId, CancellationToken cancellationToken)
    {
        var application = new CustomOpenIddictEntityFrameworkCoreApplication()
        {
            ClientId = clientId,
            DisplayName = displayName,
            TierId = tierId,
            Permissions = GetPermissions()
        };

        await _applicationManager.CreateAsync(application, clientSecret, cancellationToken);
    }

    public async Task Update(CustomOpenIddictEntityFrameworkCoreApplication client, CancellationToken cancellationToken)
    {
        await _applicationManager.UpdateAsync(client, cancellationToken);
    }

    public async Task Delete(string clientId, CancellationToken cancellationToken)
    {
        var client = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        await _applicationManager.DeleteAsync(client, cancellationToken);
    }

    public async Task ChangeClientSecret(CustomOpenIddictEntityFrameworkCoreApplication client, string clientSecret, CancellationToken cancellationToken)
    {
        await _applicationManager.UpdateAsync(client, clientSecret, cancellationToken);
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
}
