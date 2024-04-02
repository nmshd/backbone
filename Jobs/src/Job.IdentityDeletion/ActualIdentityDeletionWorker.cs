using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;
using CSharpFunctionalExtensions;
using MediatR;
using DeletionStartsNotification = Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess.DeletionStartsNotification;

namespace Backbone.Job.IdentityDeletion;

public class ActualIdentityDeletionWorker : IHostedService
{
    private readonly IEventBus _eventBus;
    private readonly IHostApplicationLifetime _host;
    private readonly IEnumerable<IIdentityDeleter> _identityDeleters;
    private readonly IMediator _mediator;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ILogger<ActualIdentityDeletionWorker> _logger;

    public ActualIdentityDeletionWorker(IHostApplicationLifetime host,
        IEnumerable<IIdentityDeleter> identityDeleters,
        IMediator mediator,
        IPushNotificationSender pushNotificationSender,
        IEventBus eventBus,
        ILogger<ActualIdentityDeletionWorker> logger)
    {
        _host = host;
        _identityDeleters = identityDeleters;
        _mediator = mediator;
        _pushNotificationSender = pushNotificationSender;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartProcessing(cancellationToken);

        _host.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task StartProcessing(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new TriggerRipeDeletionProcessesCommand(), cancellationToken);

        var addressesWithTriggeredDeletionProcesses = response.Results.Where(x => x.Value.IsSuccess).Select(x => x.Key);
        var erroringDeletionTriggers = response.Results.Where(x => x.Value.IsFailure);

        await ExecuteDeletion(addressesWithTriggeredDeletionProcesses, cancellationToken);
        LogErroringDeletionTriggers(erroringDeletionTriggers);
    }

    private async Task ExecuteDeletion(IEnumerable<IdentityAddress> addresses, CancellationToken cancellationToken)
    {
        foreach (var identityAddress in addresses)
        {
            await ExecuteDeletion(cancellationToken, identityAddress);
        }
    }

    private async Task ExecuteDeletion(CancellationToken cancellationToken, IdentityAddress identityAddress)
    {
        await NotifyIdentityAboutStartingDeletion(cancellationToken, identityAddress);
        await NotifyRelationshipsAboutStartingDeletion(identityAddress, cancellationToken);
        await Delete(identityAddress);
    }

    private async Task NotifyIdentityAboutStartingDeletion(CancellationToken cancellationToken, IdentityAddress identityAddress)
    {
        await _pushNotificationSender.SendNotification(identityAddress, new DeletionStartsNotification(), cancellationToken);
    }

    private async Task NotifyRelationshipsAboutStartingDeletion(IdentityAddress identityAddress, CancellationToken cancellationToken)
    {
        var relationships = await _mediator.Send(new FindRelationshipsOfIdentityQuery(identityAddress), cancellationToken);

        foreach (var relationship in relationships)
        {
            _eventBus.Publish(new PeerIdentityDeletedIntegrationEvent(relationship.Id, identityAddress));
        }
    }

    private async Task Delete(IdentityAddress identityAddress)
    {
        foreach (var identityDeleter in _identityDeleters)
        {
            await identityDeleter.Delete(identityAddress);
        }
    }

    private void LogErroringDeletionTriggers(IEnumerable<KeyValuePair<IdentityAddress, UnitResult<DomainError>>> erroringDeletionTriggers)
    {
        foreach (var erroringDeletion in erroringDeletionTriggers)
        {
            _logger.ErrorWhenTriggeringDeletionProcessForIdentity(erroringDeletion.Key, erroringDeletion.Value.Error.Code, erroringDeletion.Value.Error.Message);
        }
    }
}

internal static partial class ActualIdentityDeletionWorkerLogs
{
    [LoggerMessage(
        EventId = 390931,
        EventName = "ActualIdentityDeletionWorker.ErrorWhenTriggeringDeletionProcessForIdentity",
        Level = LogLevel.Error,
        Message = "There was an error when trying to trigger the deletion process for the identity with the address {identityAddress}. Error code: '{errorCode}. Error message: {errorMessage}...")]
    public static partial void ErrorWhenTriggeringDeletionProcessForIdentity(this ILogger logger, IdentityAddress identityAddress, string errorCode, string errorMessage);
}
