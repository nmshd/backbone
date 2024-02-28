using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleDeletionProcesses;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
//using DeletionStartsNotification = Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess.DeletionStartsNotification;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Job.IdentityDeletion;
public class CancelDeletionProcessWorker : IHostedService
{
    private readonly IEventBus _eventBus;
    private readonly IHostApplicationLifetime _host;
    private readonly IMediator _mediator;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ILogger<CancelDeletionProcessWorker> _logger;

    public CancelDeletionProcessWorker(IHostApplicationLifetime host,
        IMediator mediator,
        IPushNotificationSender pushNotificationSender,
        IEventBus eventBus,
        ILogger<CancelDeletionProcessWorker> logger)
    {
        _host = host;
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
        var identities = (await _mediator.Send(new CancelStaleDeletionProcessesCommand(), cancellationToken)).IdentityDeletionProcesses;

        foreach (var identity in identities)
        {
            // todo: move this logic to handler
            var staleDeletionProcess = identity.DeletionProcesses.First(d => d.Status == DeletionProcessStatus.WaitingForApproval);
            identity.CancelStaleDeletionProcess(staleDeletionProcess.Id);
            // todo: also update repository

            // todo: sending notifications by using event buss
            //await _pushNotificationSender.SendNotification
            //    (identity.Address, new DeletionProcessCanceledPushNotification(), cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
