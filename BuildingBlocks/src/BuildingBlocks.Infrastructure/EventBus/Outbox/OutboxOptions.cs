namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public sealed class OutboxOptions
{
    public int BatchSize { get; set; } = 50;
    public int Parallelism { get; set; } = 4;
    public int PollIntervalMilliseconds { get; set; } = 1000;
    public int LeaseSeconds { get; set; } = 120;
    public int MaxAttempts { get; set; } = 20;
    public int InitialRetrySeconds { get; set; } = 2;
    public int MaxRetrySeconds { get; set; } = 3600;

    public TimeSpan RetryDelay(int attempts) => TimeSpan.FromSeconds(
        Math.Min(MaxRetrySeconds, InitialRetrySeconds * Math.Pow(2, Math.Min(30, attempts - 1))) * (0.5 + Random.Shared.NextDouble() * 0.5));
}
