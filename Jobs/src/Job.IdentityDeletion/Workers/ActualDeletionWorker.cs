using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
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
    private readonly IDeletionProcessLogger _deletionProcessLogger;
    private readonly List<IIdentityDeleter> _remainingIdentityDeleters;
    private readonly IIdentityDeleter? _deviceIdentityDeleter;

    public ActualDeletionWorker(
        IHostApplicationLifetime host,
        IEnumerable<IIdentityDeleter> identityDeleters,
        IMediator mediator,
        IPushNotificationSender pushNotificationSender,
        ILogger<ActualDeletionWorker> logger,
        IDeletionProcessLogger deletionProcessLogger)
    {
        _host = host;
        _remainingIdentityDeleters = identityDeleters.ToList();
        _deviceIdentityDeleter = _remainingIdentityDeleters.First(i => i.GetType() == typeof(Modules.Devices.Application.Identities.IdentityDeleter));
        _remainingIdentityDeleters.Remove(_deviceIdentityDeleter);
        _mediator = mediator;
        _pushNotificationSender = pushNotificationSender;
        _logger = logger;
        _deletionProcessLogger = deletionProcessLogger;
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
        await Delete(identityAddress);
        await DeleteDeviceIdentity(identityAddress);
    }

    private async Task NotifyIdentityAboutStartingDeletion(CancellationToken cancellationToken, IdentityAddress identityAddress)
    {
        await _pushNotificationSender.SendNotification(identityAddress, new DeletionStartsPushNotification(), cancellationToken);
    }

    private async Task Delete(IdentityAddress identityAddress)
    {
        foreach (var identityDeleter in _remainingIdentityDeleters)
        {
            await identityDeleter.Delete(identityAddress, _deletionProcessLogger);
        }
    }

    private async Task DeleteDeviceIdentity(IdentityAddress identityAddress)
    {
        await _deviceIdentityDeleter!.Delete(identityAddress, _deletionProcessLogger);
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
