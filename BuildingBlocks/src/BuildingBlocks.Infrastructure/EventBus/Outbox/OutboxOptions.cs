using System.ComponentModel.DataAnnotations;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.Outbox;

public sealed class OutboxOptions : IValidatableObject
{
    [Range(1, 1000)]
    public int BatchSize { get; set; } = 50;

    [Range(1, 64)]
    public int Parallelism { get; set; } = 4;

    [Range(10, int.MaxValue)]
    public int PollIntervalMilliseconds { get; set; } = 1000;

    [Range(15, int.MaxValue)]
    public int LeaseSeconds { get; set; } = 120;

    [Range(1, int.MaxValue)]
    public int MaxAttempts { get; set; } = 20;

    [Range(1, int.MaxValue)]
    public int InitialRetrySeconds { get; set; } = 2;

    [Range(1, int.MaxValue)]
    public int MaxRetrySeconds { get; set; } = 3600;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaxRetrySeconds < InitialRetrySeconds)
            yield return new ValidationResult($"{nameof(MaxRetrySeconds)} must be greater than or equal to {nameof(InitialRetrySeconds)}.", [nameof(MaxRetrySeconds)]);
    }

    public TimeSpan RetryDelay(int attempts) => TimeSpan.FromSeconds(
        Math.Min(MaxRetrySeconds, InitialRetrySeconds * Math.Pow(2, Math.Min(30, attempts - 1))) * (0.5 + Random.Shared.NextDouble() * 0.5));
}
