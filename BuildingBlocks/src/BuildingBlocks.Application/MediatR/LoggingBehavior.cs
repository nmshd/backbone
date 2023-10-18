using System.Diagnostics;
using Enmeshed.BuildingBlocks.Infrastructure.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enmeshed.BuildingBlocks.Application.MediatR;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly EventId EVENT_ID_EXECUTION_TIME = new(1000, "ExecutionTimeTooHigh");

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
        var response = await next();
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

        if (_watch.ElapsedMilliseconds > 1000)
            _logger.HandledMediatorRequestWarning(typeof(TRequest).Name, _watch.ElapsedMilliseconds);
        else
            _logger.HandledMediatorRequestInformation(typeof(TRequest).Name, _watch.ElapsedMilliseconds);
    }
}

internal static partial class LoggingBehaviorLogs
{
    [LoggerMessage(
        EventId = 724322,
        EventName = "LoggingBehavior.HandledRequestInformation",
        Level = LogLevel.Information,
        Message = "Handled '{requestName}' ('{timeElapsed}' ms).")]
    public static partial void HandledMediatorRequestInformation(this ILogger logger, string requestName, long timeElapsed);

    [LoggerMessage(
        EventId = 724322,
        EventName = "LoggingBehavior.HandledRequestInformation",
        Level = LogLevel.Warning,
        Message = "Handled '{requestName}' ('{timeElapsed}' ms).")]
    public static partial void HandledMediatorRequestWarning(this ILogger logger, string requestName, long timeElapsed);
}
