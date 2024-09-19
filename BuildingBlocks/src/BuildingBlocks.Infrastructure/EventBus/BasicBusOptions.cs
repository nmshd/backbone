namespace Backbone.BuildingBlocks.Infrastructure.EventBus;
public class BasicBusOptions
{
    public string SubscriptionClientName { get; set; } = null!;
    public HandlerRetryBehavior HandlerRetryBehavior { get; set; } = null!;
}
