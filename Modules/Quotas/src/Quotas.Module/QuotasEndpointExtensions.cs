using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Quotas.Module.Features.Identities.ListQuotasForIdentity;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Quotas.Module;

public static class QuotasEndpointExtensions
{
    public static IEndpointRouteBuilder MapQuotasEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedEndpointGroup("Quotas", "Quotas", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        group.MapListQuotasForIdentityEndpoint();
        return endpoints;
    }
}
