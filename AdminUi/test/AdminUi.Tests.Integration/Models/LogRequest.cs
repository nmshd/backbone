namespace Backbone.AdminUi.Tests.Integration.Models;
public class LogRequest
{
    public required LogLevel LogLevel { get; set; }
    public required string Category { get; set; }
    public required string MessageTemplate { get; set; }
    public object[]? Arguments { get; set; }
}

public enum LogLevel
{
    Trace,
    Debug,
    Information,
    Log,
    Warning,
    Error,
    Critical
}
