using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsInLanguage;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Announcements.Module;

public static class AnnouncementsEndpointExtensions
{
    public static IEndpointRouteBuilder MapAnnouncementsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedEndpointGroup("Announcements", "Announcements", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        group.MapListAnnouncementsInLanguageEndpoint();
        return endpoints;
    }
}
