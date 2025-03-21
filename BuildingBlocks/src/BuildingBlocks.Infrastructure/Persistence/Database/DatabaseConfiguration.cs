﻿using System.ComponentModel.DataAnnotations;

namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

public class DatabaseConfiguration
{
    [Required]
    [RegularExpression($"{IServiceCollectionExtensions.SQLSERVER}|{IServiceCollectionExtensions.POSTGRES}")]
    public string Provider { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public bool EnableHealthCheck { get; set; } = true;

    public int CommandTimeout { get; set; } = 20;

    public RetryConfiguration RetryConfiguration { get; set; } = new();
}

public class RetryConfiguration
{
    public byte MaxRetryCount { get; set; } = 15;
    public int MaxRetryDelayInSeconds { get; set; } = 30;
}
