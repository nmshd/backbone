using Backbone.Tooling.Extensions;
using Serilog.Context;

namespace Backbone.BuildingBlocks.Infrastructure.CorrelationIds;

public static class CustomLogContext
{
    private static readonly AsyncLocal<string> CORRELATION_ID = new();

    public static IDisposable SetCorrelationId(string correlationId)
    {
        CORRELATION_ID.Value = correlationId;
        return LogContext.PushProperty("CorrelationId", correlationId);
    }

    public static string GetCorrelationId()
    {
        if (CORRELATION_ID.Value.IsNullOrEmpty())
            CORRELATION_ID.Value = GenerateCorrelationId();

        return CORRELATION_ID.Value;
    }

    public static string GenerateCorrelationId()
    {
        return Guid.NewGuid().ToString();
    }
}
