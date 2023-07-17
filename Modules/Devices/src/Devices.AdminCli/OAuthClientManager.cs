using Enmeshed.BuildingBlocks.Domain;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Backbone.Modules.Devices.AdminCli;

public class CreatedClientDTO
{
    public CreatedClientDTO(string clientId, string name, string clientSecret)
    {
        ClientId = clientId;
        Name = name;
        ClientSecret = clientSecret;
    }

    public string ClientId { get; init; }
    public string Name { get; init; }
    public string ClientSecret { get; init; }
}

public class ClientDTO
{
    public ClientDTO(string clientId, string displayName)
    {
        ClientId = clientId;
        DisplayName = displayName;
    }

    public string ClientId { get; set; }
    public string DisplayName { get; set; }
}

public class OAuthClientManager
{
    private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> _applicationManager;

    public OAuthClientManager(
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager)
    {
        _applicationManager = applicationManager;
    }

    public async Task<CreatedClientDTO> Create(string? clientId, string? name, string? clientSecret)
    {
        clientSecret = string.IsNullOrEmpty(clientSecret) ? Password.Generate(30) : clientSecret;
        clientId = string.IsNullOrEmpty(clientId) ? ClientIdGenerator.Generate() : clientId;
        name = string.IsNullOrEmpty(name) ? clientId : name;

        if (await _applicationManager.FindByClientIdAsync(clientId) != null)
        {
            throw new Exception($"A client with the id '{clientId}' already exists.");
        }

        var managerResult = await _applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            ClientSecret = clientSecret, // Note: the default implementation automatically hashes the client secret before storing it in the database, for security reasons.
            DisplayName = name,
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.Password
            }
        });

        if (managerResult == null)
            throw new Exception($"Failed to create the client: '{name}'");

        return new CreatedClientDTO(clientId, name, clientSecret);
    }

    public async Task Delete(string clientId)
    {
        if (string.IsNullOrEmpty(clientId))
            throw new ArgumentNullException(nameof(clientId));

        var client = await _applicationManager.FindByClientIdAsync(clientId) ?? throw new ArgumentException($"A client with the client id '{clientId}' does not exist.");
        await _applicationManager.DeleteAsync(client);
    }

    public IAsyncEnumerable<ClientDTO> GetAll()
    {
        return _applicationManager.ListAsync(applications => applications.Select(c => new ClientDTO(c.ClientId, c.DisplayName)), CancellationToken.None);
    }
}

public static class ClientIdGenerator
{
    public const int MAX_LENGTH = 20;
    public const int PREFIX_LENGTH = 3;
    public const int MAX_LENGTH_WITHOUT_PREFIX = MAX_LENGTH - PREFIX_LENGTH;
    public const string PREFIX = "CLT";

    private static readonly char[] VALID_CHARS =
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    public static string Generate()
    {
        var stringValue = StringUtils.Generate(VALID_CHARS, MAX_LENGTH_WITHOUT_PREFIX);
        return PREFIX + stringValue;
    }
}
