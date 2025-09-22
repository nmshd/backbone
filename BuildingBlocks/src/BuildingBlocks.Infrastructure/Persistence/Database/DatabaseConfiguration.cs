using System.ComponentModel.DataAnnotations;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public class DatabaseConfiguration
{
    public const string SQLSERVER = "SqlServer";
    public const string POSTGRES = "Postgres";

    [Required]
    [RegularExpression($"{SQLSERVER}|{POSTGRES}")]
    public required string Provider { get; init; }

    [Required]
    [MinLength(1)]
    public required string ConnectionString { get; init; }

    public bool EnableHealthCheck { get; init; } = true;

    public int CommandTimeout { get; init; } = 20;

    [Required]
    public RetryConfiguration RetryConfiguration { get; init; } = new();
}

public class RetryConfiguration
{
    public byte MaxRetryCount { get; init; } = 15;
    public int MaxRetryDelayInSeconds { get; init; } = 30;
}
