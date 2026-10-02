using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Tags.Module.Features.Tags.ListTags;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Tags.Module;

public static class TagsEndpointExtensions
{
    public static IEndpointRouteBuilder MapTagsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedEndpointGroup("Tags", "Tags", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        group.MapListTagsEndpoint();
        return endpoints;
    }
}
