using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Serilog.Context;
using Serilog.Core.Enrichers;
using Serilog.Core;

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
        try
        {
            var deviceId = _userContext.GetDeviceId();
            var identityAddress = _userContext.GetAddress();

            ILogEventEnricher[] enrichers =
            {
                new PropertyEnricher("deviceId", deviceId),
                new PropertyEnricher("identityAddress", identityAddress)
            };

            using (LogContext.Push(enrichers))
            {
                await _next.Invoke(context);
            }
        }
        catch (Exception)
        {
            await _next.Invoke(context);
        }
    }
}

