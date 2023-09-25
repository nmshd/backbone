using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using OpenIddict.EntityFrameworkCore.Models;

namespace Backbone.Modules.Devices.Infrastructure.OpenIddict;
public class CustomOpenIddictEntityFrameworkCoreApplication : OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>
{
    public TierId DefaultTier { get; set; }
}

