namespace Backbone.BuildingBlocks.Infrastructure.Persistence.Database;
public class DbOptions
{
    public string Provider { get; set; }
    public string ConnectionString { get; set; }
    public RetryOptions RetryOptions { get; set; } = new();
}

public class RetryOptions
{
    public byte MaxRetryCount { get; set; } = 15;
    public int MaxRetryDelayInSeconds { get; set; } = 30;
}
