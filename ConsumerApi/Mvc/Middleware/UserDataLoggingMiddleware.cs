using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Microsoft.AspNetCore;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace Backbone.ConsumerApi.Mvc.Middleware;

public class UserDataLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IUserContext _userContext;

    public UserDataLoggingMiddleware(RequestDelegate next, IUserContext userContext)
    {
        _next = next;
        _userContext = userContext;
    }

    public async Task Invoke(HttpContext context)
    {
        var deviceId = _userContext.GetDeviceIdOrNull();
        var identityAddress = _userContext.GetAddressOrNull();
        var username = _userContext.GetUsernameOrNull() ?? context.GetOpenIddictServerRequest()?.Username;

        ILogEventEnricher[] enrichers =
        [
            new PropertyEnricher("deviceId", deviceId ?? ""),
            new PropertyEnricher("identityAddress", identityAddress ?? ""),
            new PropertyEnricher("username", username ?? "")
        ];

        using (LogContext.Push(enrichers))
        {
            await _next.Invoke(context);
        }
    }
}

