using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;

namespace Backbone.BuildingBlocks.API.Mvc.Middleware;

public class UserContextBaggageMiddleware
{
    private const string DEVICE_ID = "deviceId";
    private const string IDENTITY_ADDRESS = "identityAddress";
    private const string CLIENT_ID = "clientId";
    private const string USERNAME = "username";

    private readonly RequestDelegate _next;

    public UserContextBaggageMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserContext userContext)
    {
        var baggageItems = new List<KeyValuePair<string, string?>>();

        AddIfNotNull(baggageItems, DEVICE_ID, userContext.GetDeviceIdOrNull()?.Value);
        AddIfNotNull(baggageItems, IDENTITY_ADDRESS, userContext.GetAddressOrNull()?.Value);
        AddIfNotNull(baggageItems, CLIENT_ID, userContext.GetClientIdOrNull());
        AddIfNotNull(baggageItems, USERNAME, userContext.GetUsernameOrNull());

        if (baggageItems.Count > 0)
            Baggage.SetBaggage(baggageItems);

        await _next(context);
    }

    private static void AddIfNotNull(List<KeyValuePair<string, string?>> baggageItems, string key, string? value)
    {
        if (value == null)
            return;

        baggageItems.Add(new KeyValuePair<string, string?>(key, value));
    }
}
