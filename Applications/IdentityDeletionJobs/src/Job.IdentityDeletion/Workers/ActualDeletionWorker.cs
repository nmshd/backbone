using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.HandleCompletedDeletionProcess;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using CSharpFunctionalExtensions;
using MediatR;

namespace Backbone.Job.IdentityDeletion.Workers;

public class ActualDeletionWorker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IMediator _mediator;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ILogger<ActualDeletionWorker> _logger;
    private readonly List<IIdentityDeleter> _identityDeleters;

    public ActualDeletionWorker(
        IHostApplicationLifetime host,
        IEnumerable<IIdentityDeleter> identityDeleters,
        IMediator mediator,
        IPushNotificationSender pushNotificationSender,
        ILogger<ActualDeletionWorker> logger)
    {
        _host = host;
        _identityDeleters = identityDeleters.ToList();
        _mediator = mediator;
        _pushNotificationSender = pushNotificationSender;
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
            await ExecuteDeletion(identityAddress, cancellationToken);
        }
    }

    private async Task ExecuteDeletion(IdentityAddress identityAddress, CancellationToken cancellationToken)
    {
        await NotifyIdentityAboutStartingDeletion(identityAddress, cancellationToken);
        await Delete(identityAddress);
    }

    private async Task NotifyIdentityAboutStartingDeletion(IdentityAddress identityAddress, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(
            new DeletionStartsPushNotification(),
            SendPushNotificationFilter.AllDevicesOf(identityAddress),
            cancellationToken);
    }

    private async Task Delete(IdentityAddress identityAddress)
    {
        var identity = await _mediator.Send(new GetIdentityQuery(identityAddress.Value));

        foreach (var identityDeleter in _identityDeleters)
        {
            await identityDeleter.Delete(identityAddress);
        }

        var usernames = identity.Devices.Select(d => d.Username);

        await _mediator.Send(new HandleCompletedDeletionProcessCommand(identityAddress, usernames));
    }

    private void LogErroringDeletionTriggers(IEnumerable<KeyValuePair<IdentityAddress, UnitResult<DomainError>>> erroringDeletionTriggers)
    {
        foreach (var erroringDeletion in erroringDeletionTriggers)
        {
            _logger.ErrorWhenTriggeringDeletionProcessForIdentity(erroringDeletion.Value.Error.Code, erroringDeletion.Value.Error.Message);
        }
    }
}

internal static partial class ActualIdentityDeletionWorkerLogs
{
    [LoggerMessage(
        EventId = 390931,
        EventName = "ActualIdentityDeletionWorker.ErrorWhenTriggeringDeletionProcessForIdentity",
        Level = LogLevel.Error,
        Message = "There was an error when trying to trigger the deletion process for the identity. Error code: '{errorCode}. Error message: {errorMessage}...")]
    public static partial void ErrorWhenTriggeringDeletionProcessForIdentity(this ILogger logger, string errorCode, string errorMessage);
}
