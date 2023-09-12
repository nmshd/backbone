namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus;
public class BasicBusOptions
{
#pragma warning disable CS8618
    public string SubscriptionClientName { get; set; }
    public int NumberOfRetries { get; set; } = 5;
    public int MinimumBackoff { get; set; } = 2;
    public int MaximumBackoff { get; set; } = 120;
#pragma warning restore CS8618
}
