﻿namespace AdminUi.Tests.Integration.Models;
public class LogRequest
{
    public LogLevel LogLevel { get; set; }
    public string Category { get; set; }
    public string MessageTemplate { get; set; }
    public object[] Arguments { get; set; }
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
