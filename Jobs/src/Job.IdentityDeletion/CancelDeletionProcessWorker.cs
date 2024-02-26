using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerStaleDeletionProcesses;


//using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
//using Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;
using MediatR;
//using DeletionStartsNotification = Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess.DeletionStartsNotification;

namespace Backbone.Job.IdentityDeletion;
public class CancelDeletionProcessWorker : IHostedService
{
    private readonly IEventBus _eventBus;
    private readonly IHostApplicationLifetime _host;
    private readonly IMediator _mediator;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ILogger<CancelDeletionProcessWorker> _logger;
    private readonly IReadOnlyList<IdentityDeletionProcess> _deletionProcesses;

    public CancelDeletionProcessWorker(IHostApplicationLifetime host,
        IMediator mediator,
        IPushNotificationSender pushNotificationSender,
        IEventBus eventBus,
        ILogger<CancelDeletionProcessWorker> logger,
        IReadOnlyList<IdentityDeletionProcess> deletionProcesses)
    {
        _host = host;
        _mediator = mediator;
        _pushNotificationSender = pushNotificationSender;
        _eventBus = eventBus;
        _logger = logger;
        _deletionProcesses = deletionProcesses;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartProcessing(cancellationToken);

        _host.StopApplication();
    }

    private async Task StartProcessing(CancellationToken cancellationToken)
    {
        foreach (var identityDeletionProcess in _deletionProcesses)
        {
            if (identityDeletionProcess.Status == DeletionProcessStatus.WaitingForApproval 
                && identityDeletionProcess.CreatedAt.AddDays(IdentityDeletionConfiguration.MaxApprovalTime) > DateTime.UtcNow)
            {
                var identities = 
                    await _mediator.Send(new TriggerStaleDeletionProcessesCommand("", identityDeletionProcess.Id), cancellationToken);
            }
        }

        //var identities = await _mediator.Send(new TriggerStaleDeletionProcessesCommand(), cancellationToken);

        //foreach (var identity in identities.Identities)
        //{
        //    foreach (var identityDeletionProcess in _deletionProcesses)
        //    {
        //        if (identityDeletionProcess.CreatedAt < DateTime.UtcNow)
        //        {
        //            await _pushNotificationSender.SendNotification
        //                (identity.Address, new DeletionCanceledNotification(IdentityDeletionConfiguration.DeletionCanceledNotification.Message), cancellationToken);
        //            identityDeletionProcess.CancelAutomatically(identity.Address);
        //        }
        //    }
        //}
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

[NotificationText(Title = "Deletion process canceled", Body = "Your Identity deletion process is canceled due to no response within the approval period")]
public record DeletionCanceledNotification
{
    public DeletionCanceledNotification(string message) => GetType().GetCustomAttribute<NotificationTextAttribute>().Body = message;
}
