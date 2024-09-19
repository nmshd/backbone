namespace Backbone.Tooling.Extensions;

public static class EnvironmentVariables
{
    public static readonly bool DEBUG_PERFORMANCE = !Environment.GetEnvironmentVariable("DEBUG_PERFORMANCE").IsNullOrEmpty();
}
