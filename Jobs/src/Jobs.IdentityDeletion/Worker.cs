using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;
using MediatR;
using Microsoft.Extensions.Logging;
using DeletionStartsNotification = Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess.DeletionStartsNotification;

namespace Backbone.Job.IdentityDeletion;
public class Worker : IHostedService
{
    private readonly IEventBus _eventBus;
    private readonly IHostApplicationLifetime _host;
    private readonly IEnumerable<IIdentityDeleter> _identityDeleters;
    private readonly IMediator _mediator;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ILogger<Worker> _logger;

    public Worker(IHostApplicationLifetime host,
                    IEnumerable<IIdentityDeleter> identityDeleters,
                    IMediator mediator,
                    IPushNotificationSender pushNotificationSender,
                    IEventBus eventBus,
                    ILogger<Worker> logger)
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

    public async Task StartProcessing(CancellationToken cancellationToken)
    {
        var triggeredForDeletionIdentityAddresses = await _mediator.Send(new TriggerRipeDeletionProcessesCommand(), cancellationToken);

        var successfulDeletionTriggers = triggeredForDeletionIdentityAddresses.DeletedIdentityAddresses.Where(x => x.Value.IsSuccess);
        var erroringDeletionTriggers = triggeredForDeletionIdentityAddresses.DeletedIdentityAddresses.Where(x => x.Value.IsFailure);

        foreach (var identityAddress in successfulDeletionTriggers.Select(x => x.Key))
        {
            await _pushNotificationSender.SendNotification(identityAddress, new DeletionStartsNotification(), cancellationToken);

            var relationships = await _mediator.Send(new FindRelationshipsOfIdentityQuery(identityAddress), cancellationToken);

            foreach (var relationship in relationships)
            {
                _eventBus.Publish(new PeerIdentityDeletedIntegrationEvent(relationship.Id, identityAddress));
            }

            foreach (var identityDeleter in _identityDeleters)
            {
                await identityDeleter.Delete(identityAddress);
            }
        }

        foreach (var erroringDeletion in erroringDeletionTriggers)
        {
            _logger.LogError(
                new DomainException(erroringDeletion.Value.Error),
                "There was an error while triggering a deletion process.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
