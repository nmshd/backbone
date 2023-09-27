using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Azure;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace ConsumerApi.Mvc.Middleware;

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

        List<ILogEventEnricher> enrichers = new()
        {
            new PropertyEnricher("deviceId", deviceId ?? ""),
            new PropertyEnricher("identityAddress", identityAddress ?? "")
        };

        if (deviceId is null || identityAddress is null)
        {
            enrichers.Add(new PropertyEnricher("username", context.GetOpenIddictServerRequest()?.Username));
        }

        using (LogContext.Push(enrichers.ToArray()))
        {
            await _next.Invoke(context);
        }
    }
}

