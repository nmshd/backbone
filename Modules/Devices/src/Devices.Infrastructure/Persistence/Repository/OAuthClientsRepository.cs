using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Core;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class OAuthClientsRepository : IOAuthClientsRepository
{
    private readonly OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> _applicationManager;
    private readonly IQueryable<Identity> _readonlyIdentities;
    private readonly Dictionary<string, CustomOpenIddictEntityFrameworkCoreApplication> _trackedApplications;

    public OAuthClientsRepository(OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> applicationManager, DevicesDbContext dbContext)
    {
        _applicationManager = applicationManager;
        _readonlyIdentities = dbContext.Identities.AsNoTracking();
        _trackedApplications = new Dictionary<string, CustomOpenIddictEntityFrameworkCoreApplication>();
    }

    public async Task<IEnumerable<OAuthClient>> List(CancellationToken cancellationToken, bool track = false)
    {
        var applications = await _applicationManager.ListAsync(cancellationToken: cancellationToken).ToListAsync(cancellationToken);

        if (track)
            Track(applications);

        var oAuthClients = applications.Select(a => a.ToModel());

        return oAuthClients;
    }

    public async Task<Dictionary<string, int>> CountIdentities(List<string> clientIds, CancellationToken cancellationToken)
    {
        var identityCounts = await _readonlyIdentities
            .Where(i => i.ClientId != null && clientIds.Contains(i.ClientId))
            .GroupBy(i => i.ClientId)
            .Select(g => new { ClientId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.ClientId!, g => g.Count, cancellationToken);

        foreach (var clientId in clientIds.Where(clientId => !identityCounts.ContainsKey(clientId)))
            identityCounts[clientId] = 0;

        return identityCounts;
    }

    public async Task<OAuthClient?> Get(string clientId, CancellationToken cancellationToken, bool track = false)
    {
        if (_trackedApplications.TryGetValue(clientId, out var trackedApplication))
        {
            return trackedApplication.ToModel();
        }

        var application = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);

        if (track)
            Track(application);

        return application?.ToModel();
    }

    public async Task<bool> Exists(string clientId, CancellationToken cancellationToken)
    {
        var client = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken);
        return client != null;
    }

    public async Task Add(OAuthClient client, string secret, CancellationToken cancellationToken)
    {
        var application = new CustomOpenIddictEntityFrameworkCoreApplication
        {
            ClientId = client.ClientId,
            DisplayName = client.DisplayName,
            DefaultTier = client.DefaultTier,
            CreatedAt = client.CreatedAt,
            MaxIdentities = client.MaxIdentities,
            Permissions = GetPermissions()
        };

        await _applicationManager.CreateAsync(application, secret, cancellationToken);
    }

    private static string GetPermissions()
    {
        var permissions = new List<string>
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
        var application = await GetApplication(client.ClientId, cancellationToken);

        application.UpdateFromModel(client);
        await _applicationManager.UpdateAsync(application, cancellationToken);
    }

    public async Task Delete(string clientId, CancellationToken cancellationToken)
    {
        var application = await GetApplication(clientId, cancellationToken);

        await _applicationManager.DeleteAsync(application, cancellationToken);
        _trackedApplications.Remove(clientId);
    }

    public async Task ChangeClientSecret(OAuthClient client, string clientSecret, CancellationToken cancellationToken)
    {
        var application = await GetApplication(client.ClientId, cancellationToken);

        await _applicationManager.UpdateAsync(application, clientSecret, cancellationToken);
    }

    private async Task<CustomOpenIddictEntityFrameworkCoreApplication> GetApplication(string clientId, CancellationToken cancellationToken)
    {
        if (!_trackedApplications.TryGetValue(clientId, out var application))
        {
            application = await _applicationManager.FindByClientIdAsync(clientId, cancellationToken) ?? throw new NotFoundException(nameof(OAuthClient));
        }

        return application;
    }

    private void Track(IEnumerable<CustomOpenIddictEntityFrameworkCoreApplication> applications)
    {
        foreach (var application in applications)
        {
            Track(application);
        }
    }

    private void Track(CustomOpenIddictEntityFrameworkCoreApplication? application)
    {
        if (application != null)
        {
            _trackedApplications[application.ClientId!] = application;
        }
    }
}
