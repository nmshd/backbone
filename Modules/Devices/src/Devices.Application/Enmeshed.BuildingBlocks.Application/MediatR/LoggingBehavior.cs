using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Enmeshed.BuildingBlocks.Application.MediatR
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private static readonly EventId EVENT_ID_EXECUTION_TIME = new(1000, "ExecutionTimeTooHigh");

        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private Stopwatch _watch;

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
            _logger.LogTrace($"Handling {typeof(TRequest).Name}");
            _watch = Stopwatch.StartNew();
        }

        private void After()
        {
            _watch!.Stop();

            var message = $"Handled {typeof(TRequest).Name} ({_watch.ElapsedMilliseconds} ms)";
            var logLevel = _watch.ElapsedMilliseconds < 1000 ? LogLevel.Information : LogLevel.Warning;

            _logger.Log(logLevel, EVENT_ID_EXECUTION_TIME, message);
        }
    }
}