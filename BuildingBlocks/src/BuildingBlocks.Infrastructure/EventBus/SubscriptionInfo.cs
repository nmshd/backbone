namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus;

public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    public class SubscriptionInfo
    {
        private SubscriptionInfo(Type handlerType, Type eventType)
        {
            HandlerType = handlerType;
            EventType = eventType;
        }

        public Type HandlerType { get; }

        public Type EventType { get; }

        public static SubscriptionInfo Typed(Type handlerType, Type eventType)
        {
            return new SubscriptionInfo(handlerType, eventType);
        }
    }
}
