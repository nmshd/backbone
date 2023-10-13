using Microsoft.Extensions.Logging;

namespace Enmeshed.Tooling.Extensions;
public static class LogEventIds
{
    public static readonly EventId EXECUTION_TIME = new(1000, "ExecutionTime");
}
