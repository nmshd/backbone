using OpenIddict.EntityFrameworkCore.Models;

namespace Backbone.Modules.Devices.Infrastructure.OpenIddict;
public class CustomOpenIddictEntityFrameworkCoreAuthorization : OpenIddictEntityFrameworkCoreAuthorization<string, CustomOpenIddictEntityFrameworkCoreApplication, CustomOpenIddictEntityFrameworkCoreToken>;
