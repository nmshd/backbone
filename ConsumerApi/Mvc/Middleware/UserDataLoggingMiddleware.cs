using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
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

        ILogEventEnricher[] enrichers =
        {
            new PropertyEnricher("deviceId", deviceId ?? ""),
            new PropertyEnricher("identityAddress", identityAddress ?? "")
        };

        using (LogContext.Push(enrichers))
        {
            await _next.Invoke(context);
        }
    }
}

