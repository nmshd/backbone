namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus;

public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    public class SubscriptionInfo
    {
        private SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType;
        }

        public Type HandlerType { get; }

        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }
    }
}