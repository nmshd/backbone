using OpenIddict.EntityFrameworkCore.Models;

namespace Backbone.Modules.Devices.Infrastructure.OpenIddict;
public class CustomOpenIddictEntityFrameworkCoreApplication : OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>
{
    public string DefaultTier { get; set; }
}

