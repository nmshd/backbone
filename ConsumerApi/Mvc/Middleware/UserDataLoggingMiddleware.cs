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
        try
        {
            var deviceId = _userContext.GetDeviceId();
            var identityAddress = _userContext.GetAddress();
            ILogEventEnricher[] enrichers =
            {
                new PropertyEnricher("deviceId", deviceId),
                new PropertyEnricher("identityAddress", identityAddress)
            };

            using var disposable = LogContext.Push(enrichers);
        }
        finally
        {
            await _next.Invoke(context);
        }
    }
}

