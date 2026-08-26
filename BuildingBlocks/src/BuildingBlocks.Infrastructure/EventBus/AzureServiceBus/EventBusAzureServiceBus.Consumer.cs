using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.BuildingBlocks.Infrastructure.CorrelationIds;
using Backbone.Tooling.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus.AzureServiceBus;

public partial class EventBusAzureServiceBus
{
    public async Task Subscribe<T, TH>()
        where T : DomainEvent
        where TH : IDomainEventHandler<T>
    {
        var eventName = typeof(T).GetEventName();
        var subscriptionName = GetSubscriptionName<TH, T>();

        await EnsureSubscriptionExists(subscriptionName);

        await RegisterSubscriptionForEvent(subscriptionName, eventName);

        var processor = _client.CreateProcessor(TOPIC_NAME, subscriptionName, _options);

        processor.ProcessMessageAsync +=
            async args =>
            {
                var messageData = args.Message.Body.ToString();
                var correlationId = args.Message.CorrelationId;

                correlationId = correlationId.IsNullOrEmpty() ? CustomLogContext.GenerateCorrelationId() : correlationId;

                using (CustomLogContext.SetCorrelationId(correlationId))
                {
                    var processedSuccessfully = await ProcessEvent<T, TH>(messageData);

                    if (processedSuccessfully)
                        await args.CompleteMessageAsync(args.Message);
                    else
                    {
                        await args.AbandonMessageAsync(args.Message);
                        _metrics.IncrementNumberOfProcessingErrors(GetSubscriptionName<TH, T>());
                        _logger.EventWasNotProcessed(args.Message.MessageId);
                    }
                }
            };

        processor.ProcessErrorAsync += ErrorHandler;

        _processors.Add(processor);
    }

    private async Task EnsureSubscriptionExists(string subscriptionName)
    {
        if (!await _adminClient.SubscriptionExistsAsync(TOPIC_NAME, subscriptionName))
        {
            _logger.LogInformation("Creating subscription on Service Bus...");

            await _adminClient.CreateSubscriptionAsync(new CreateSubscriptionOptions(TOPIC_NAME, subscriptionName)
            {
                MaxDeliveryCount = MAX_DELIVERY_COUNT,
                DeadLetteringOnMessageExpiration = true,
            });

            _logger.LogInformation("Successfully created subscription on Service Bus.");
        }

        if (await _adminClient.RuleExistsAsync(TOPIC_NAME, subscriptionName, "$Default"))
        {
            await _adminClient.DeleteRuleAsync(TOPIC_NAME, subscriptionName, "$Default");
        }
    }

    private async Task RegisterSubscriptionForEvent(string subscriptionName, string eventName)
    {
        if (!await _adminClient.RuleExistsAsync(TOPIC_NAME, subscriptionName, eventName))
        {
            _logger.LogInformation("Creating rule on subscription...");

            await _adminClient.CreateRuleAsync(TOPIC_NAME, subscriptionName,
                new CreateRuleOptions
                {
                    Filter = new CorrelationRuleFilter { Subject = eventName },
                    Name = eventName
                });

            _logger.LogInformation("Successfully created rule on subscription.");
        }
    }

    public async Task StartConsuming(CancellationToken cancellationToken)
    {
        await RegisterSubscriptionClientMessageHandler(cancellationToken);
    }

    private async Task RegisterSubscriptionClientMessageHandler(CancellationToken cancellationToken)
    {
        foreach (var processor in _processors)
        {
            await processor.StartProcessingAsync(cancellationToken);
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        var ex = args.Exception;
        var context = args.ErrorSource;

        _logger.ErrorHandlingMessage(context, ex);

        return Task.CompletedTask;
    }

    private async Task<bool> ProcessEvent<TEvent, THandler>(string message) where TEvent : DomainEvent where THandler : IDomainEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);

        var domainEvent = JsonSerializer.Deserialize<TEvent>(message)!;
        var concreteType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            if (scope.ServiceProvider.GetService(typeof(THandler)) is not IDomainEventHandler handler)
                throw new Exception($"Domain event handler '{typeof(THandler).FullName}' could not be resolved from dependency container or it does not implement {nameof(IDomainEventHandler)}.");

            var startedAt = Stopwatch.GetTimestamp();
            await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!;
            _metrics.TrackEventProcessingDuration(startedAt, GetSubscriptionName<THandler, TEvent>());

            _metrics.IncrementNumberOfHandledEvents(GetSubscriptionName<THandler, TEvent>());
        }
        catch (Exception ex)
        {
            _logger.ErrorWhileProcessingDomainEvent(domainEvent.DomainEventId, ex);
            return false;
        }

        return true;
    }

    public async Task StopConsuming(CancellationToken cancellationToken)
    {
        foreach (var processor in _processors)
        {
            await processor.StopProcessingAsync(cancellationToken);
        }
    }
}
