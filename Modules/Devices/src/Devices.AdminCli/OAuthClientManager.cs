using Devices.Domain.Entities;
using Devices.Infrastructure.Persistence.Database;
using Enmeshed.StronglyTypedIds;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Devices.AdminCli;

public class CreatedClientDTO
{
    public CreatedClientDTO(string clientId, string name, string clientSecret, int accessTokenLifetime)
    {
        ClientId = clientId;
        Name = name;
        ClientSecret = clientSecret;
        AccessTokenLifetime = accessTokenLifetime;
    }

    public string ClientId { get; init; }
    public string Name { get; init; }
    public string ClientSecret { get; init; }
    public int AccessTokenLifetime { get; init; }
}

public class ClientDTO
{
    public ClientDTO(string clientId, string name, int accessTokenLifetime)
    {
        ClientId = clientId;
        Name = name;
        AccessTokenLifetime = accessTokenLifetime;
    }

    public string ClientId { get; set; }
    public string Name { get; set; }
    public int AccessTokenLifetime { get; set; }
}

public class OAuthClientManager
{
    //private readonly ConfigurationDbContext _dbContext;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _applicationDbContext;

    private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> _manager;

    public OAuthClientManager(
        //ConfigurationDbContext dbContext,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> manager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext applicationDbContext)
    {
        //_dbContext = dbContext;
        _manager = manager;
        _userManager = userManager;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<CreatedClientDTO> Create(string? clientId, string? name, string? clientSecret, int? accessTokenLifetime)
    {
        clientSecret = string.IsNullOrEmpty(clientSecret) ? Password.Generate(30) : clientSecret;
        clientId = string.IsNullOrEmpty(clientId) ? ClientIdGenerator.Generate() : clientId;
        accessTokenLifetime ??= 300;
        name = string.IsNullOrEmpty(name) ? clientId : name;

        if (await _manager.FindByClientIdAsync(clientId) is null)
        {
            OpenIddictEntityFrameworkCoreApplication managerResult = await _manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                DisplayName = name,
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.Password,
                    Permissions.GrantTypes.RefreshToken
                }
            });

            if (managerResult == null)
            {
                throw new Exception($"Failed to create the client: '{name}'");
            }
        }

        //ApplicationUser user = new ApplicationUser
        //{
        //    Id = clientId,
        //    UserName = name,
        //    Device = new Device(new Identity(clientId,
        //        IdentityAddress.Create(new byte[] { 3, 3, 3, 3, 3 }, "id1"),
        //        new byte[] { 3, 3, 3, 3, 3 }, 1
        //    ))
        //};

        //IdentityResult result = await _userManager.CreateAsync(user, clientSecret);

        //if (!result.Succeeded)
        //{
        //    string[] errorList = result.Errors.Select(e => e.Description).ToArray();
        //    throw new Exception($"Failed to create the client: '{name}' with the following errors: {string.Join(", ", errorList)}");
        //}

        return new CreatedClientDTO(clientId, name, clientSecret, accessTokenLifetime.Value);
    }

    public async void Delete(string clientId)
    {
        OpenIddictEntityFrameworkCoreApplication? client = await _manager.FindByIdAsync(clientId);

        if (client == null)
            throw new Exception($"A client with the client id '{clientId}' does not exist.");

        await _manager.DeleteAsync(client);
    }

    public IAsyncEnumerable<ClientDTO> GetAll()
    {
        return _manager.ListAsync(applications => applications.Select(c => new ClientDTO(c.Id, c.DisplayName, 300)), CancellationToken.None);
    }
}

public static class ClientIdGenerator
{
    public const int MAX_LENGTH = 20;
    public const int PREFIX_LENGTH = 3;
    public const int MAX_LENGTH_WITHOUT_PREFIX = MAX_LENGTH - PREFIX_LENGTH;
    public const string PREFIX = "CLT";

    private static readonly char[] ValidChars =
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    public static string Generate()
    {
        string stringValue = StringUtils.Generate(ValidChars, MAX_LENGTH_WITHOUT_PREFIX);
        return PREFIX + stringValue;
    }
}
