using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Application.MediatR;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private Stopwatch? _watch;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Before();
        var response = await next(cancellationToken);
        After();
        return response;
    }

    private void Before()
    {
        _logger.LogTrace("Handling '{name}'", typeof(TRequest).Name);
        _watch = Stopwatch.StartNew();
    }

    private void After()
    {
        _watch!.Stop();

        _logger.HandledMediatorRequest(
            _watch.ElapsedMilliseconds > 1000 ? LogLevel.Warning : LogLevel.Information,
            typeof(TRequest).Name, _watch.ElapsedMilliseconds);
    }
}

internal static partial class LoggingBehaviorLogs
{
    [LoggerMessage(
        EventId = 724322,
        EventName = "LoggingBehavior.HandledRequest",
        Message = "Handled '{requestName}' ('{timeElapsed}' ms).")]
    public static partial void HandledMediatorRequest(this ILogger logger, LogLevel level, string requestName, long timeElapsed);
}
