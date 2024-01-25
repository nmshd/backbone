using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using OpenIddict.EntityFrameworkCore.Models;

namespace Backbone.Modules.Devices.Infrastructure.OpenIddict;

public class CustomOpenIddictEntityFrameworkCoreApplication : OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization,
    CustomOpenIddictEntityFrameworkCoreToken>
{
    public required TierId DefaultTier { get; set; }

    public required DateTime CreatedAt { get; set; }

    public int? MaxIdentities { get; set; }

    public OAuthClient ToModel()
    {
        return new OAuthClient(ClientId!, DisplayName!, DefaultTier, CreatedAt, MaxIdentities);
    }

    public void UpdateFromModel(OAuthClient client)
    {
        DefaultTier = client.DefaultTier;
        MaxIdentities = client.MaxIdentities;
    }
}
