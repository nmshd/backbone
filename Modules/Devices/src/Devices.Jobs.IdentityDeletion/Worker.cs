using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;
using MediatR;
using DeletionStartsNotification = Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess.DeletionStartsNotification;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion;
public class Worker(IHostApplicationLifetime host,
                    IEnumerable<IIdentityDeleter> identityDeleters,
                    IMediator mediator,
                    IPushNotificationSender pushNotificationSender,
                    IEventBus eventBus) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartProcessing(cancellationToken);

        host.StopApplication();
    }

    public async Task StartProcessing(CancellationToken cancellationToken)
    {
        var identities = await mediator.Send(new TriggerRipeDeletionProcessesCommand(), cancellationToken);


        foreach (var identityAddress in identities.IdentityAddresses)
        {
            await pushNotificationSender.SendNotification(identityAddress, new DeletionStartsNotification(IdentityDeletionConfiguration.DeletionStartsNotification.Message), cancellationToken);

            var relationships = (await mediator.Send(new FindRelationshipsOfIdentityCommand(identityAddress), cancellationToken)).Relationships;

            foreach (var relationship in relationships)
            {
                eventBus.Publish(new PeerIdentityDeletedIntegrationEvent(relationship.Id, identityAddress));
            }

            foreach (var identityDeleter in identityDeleters)
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
