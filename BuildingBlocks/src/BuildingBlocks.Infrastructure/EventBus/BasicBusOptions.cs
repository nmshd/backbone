namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus;
public class BasicBusOptions
{
#pragma warning disable CS8618
    public string SubscriptionClientName { get; set; }
    public int NumberOfRetries { get; set; } = 5;

    /// <summary>
    /// in milliseconds.
    /// </summary>
    public int MinimumBackoff { get; set; } = 500;

    /// <summary>
    /// in seconds.
    /// </summary>
    public int MaximumBackoff { get; set; } = 120;
#pragma warning restore CS8618
}
