using System.Reflection;
using Autofac;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Backbone.Infrastructure.EventBus;

public class EventBusInMemory : IEventBus
{
    private const string AUTOFAC_SCOPE_NAME = "event_bus";

    private record Subscription(Type Event, Type EventHandler);

    private readonly List<Subscription> _subscriptions = new();
    private readonly ILifetimeScope _autofac;

    public EventBusInMemory(ILifetimeScope autofac)
    {
        _autofac = autofac;
    }

    public void Publish(IntegrationEvent @event)
    {
        // var subscriptions = _subscriptions.Where(h => h.Event == @event.GetType());
        //
        // using var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME);
        //
        // foreach (var subscription in subscriptions)
        // {
        //     var handler = scope.ResolveOptional(subscription.EventHandler);
        //
        //
        //     if (handler == null)
        //         throw new Exception(
        //             $"The handler type {subscription.HandlerType.FullName} is not registered in the dependency container.");
        //
        //     var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(@event.GetType());
        //     (Task)concreteType.GetMethod("Handle")!.Invoke(handler, new[] { @event })!;
        //
        //     try
        //     {
        //         await handler.Handle(@event);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex,
        //             "An error occurred while processing the integration event with id '{eventId}'.",
        //             @event.IntegrationEventId);
        //         return false;
        //     }
        // }
    }

    public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        _subscriptions.Add(new Subscription(typeof(T), typeof(TH)));
    }
}

public class ContractResolverWithPrivates : CamelCasePropertyNamesContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member,
        MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);

        prop.Writable = true;

        if (prop.Writable) return prop;

        var property = member as PropertyInfo;
        if (property != null)
        {
            var hasPrivateSetter = property.GetSetMethod(true) != null;
            prop.Writable = hasPrivateSetter;
        }

        return prop;
    }
}