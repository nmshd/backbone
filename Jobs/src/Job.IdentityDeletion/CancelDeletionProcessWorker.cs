﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;
using MediatR;
using DeletionStartsNotification = Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess.DeletionStartsNotification;

namespace Backbone.Job.IdentityDeletion;
public class CancelDeletionProcessWorker : IHostedService
{
    private readonly IEventBus _eventBus;
    private readonly IHostApplicationLifetime _host;
    private readonly IEnumerable<IIdentityDeleter> _identityDeleters;
    private readonly IMediator _mediator;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ILogger<CancelDeletionProcessWorker> _logger;

    public CancelDeletionProcessWorker(IHostApplicationLifetime host,
        IEnumerable<IIdentityDeleter> identityDeleters,
        IMediator mediator,
        IPushNotificationSender pushNotificationSender,
        IEventBus eventBus,
        ILogger<CancelDeletionProcessWorker> logger)
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

    private async Task StartProcessing(CancellationToken cancellationToken)
    {
        var identities = await _mediator.Send(new TriggerRipeDeletionProcessesCommand(), cancellationToken);

        foreach (var identityAddress in identities.DeletedIdentityAddresses)
        {
            await _pushNotificationSender.SendNotification(identityAddress, new DeletionStartsNotification(IdentityDeletionConfiguration.DeletionStartsNotification.Message), cancellationToken);

            var relationships = (await _mediator.Send(new FindRelationshipsOfIdentityQuery(identityAddress), cancellationToken)).Relationships;

            foreach (var relationship in relationships)
            {
                _eventBus.Publish(new PeerIdentityDeletedIntegrationEvent(relationship.Id, identityAddress));
            }

            foreach (var identityDeleter in _identityDeleters)
            {
                await identityDeleter.Delete(identityAddress);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
