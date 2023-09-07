using OpenIddict.EntityFrameworkCore.Models;

namespace Backbone.Modules.Devices.Domain.OpenIddict;
public class CustomOpenIddictEntityFrameworkCoreApplication : OpenIddictEntityFrameworkCoreApplication<string, CustomOpenIddictEntityFrameworkCoreAuthorization, CustomOpenIddictEntityFrameworkCoreToken>
{
    public string DefaultTier { get; set; }
}

