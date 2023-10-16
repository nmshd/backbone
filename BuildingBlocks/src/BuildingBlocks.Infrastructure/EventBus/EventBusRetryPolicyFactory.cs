﻿using Polly;
using Polly.Retry;

namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus;
internal class EventBusRetryPolicyFactory
{
    static internal AsyncRetryPolicy Create(HandlerRetryBehavior handlerRetryBehavior, Action<Exception, TimeSpan> onRetry)
    {
        return Policy.Handle<Exception>()
            .WaitAndRetryAsync(
                handlerRetryBehavior.NumberOfRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(handlerRetryBehavior.MinimumBackoff, retryAttempt), handlerRetryBehavior.MaximumBackoff)),
                onRetry);
    }
}
