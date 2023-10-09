namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus;
public class BasicBusOptions
{
#pragma warning disable CS8618
    public string SubscriptionClientName { get; set; }
    public HandlerRetryBehavior HandlerRetryBehavior { get; set; }
#pragma warning restore CS8618
}
