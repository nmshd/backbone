using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Messages.Module.Features.Messages.GetMessage;
using Backbone.Modules.Messages.Module.Features.Messages.ListMessages;
using Backbone.Modules.Messages.Module.Features.Messages.SendMessage;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Messages.Module.Features.Messages;

public static class MessagesEndpointExtensions
{
    public static IEndpointRouteBuilder MapMessagesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedEndpointGroup("Messages", "Messages", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

        group.MapListMessagesEndpoint();
        group.MapGetMessageEndpoint();
        group.MapSendMessageEndpoint();

        return endpoints;
    }
}
