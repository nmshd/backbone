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
        _logger.HandleRequest(typeof(TRequest).Name, _watch.ElapsedMilliseconds);
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, string, long, Exception> HANDLED_REQUEST_WARNING =
        LoggerMessage.Define<string, long>(
            LogLevel.Warning,
            new EventId(437002, "LoggingBehavior.HandleRequestInformation"),
            "Handled '{requestName}' ('{timeElapsed}' ms)."
        );

    private static readonly Action<ILogger, string, long, Exception> HANDLED_REQUEST_INFORMATION =
        LoggerMessage.Define<string, long>(
            LogLevel.Information,
            new EventId(214089, "LoggingBehavior.HandleRequestInformation"),
            "Handled '{requestName}' ('{timeElapsed}' ms)."
        );

    public static void HandleRequest(this ILogger logger, string requestName, long timeElapsed)
    {
        if (timeElapsed > 1000)
            HANDLED_REQUEST_WARNING(logger, requestName, timeElapsed, default!);
        else
            HANDLED_REQUEST_INFORMATION(logger, requestName, timeElapsed, default!);
    }
}
