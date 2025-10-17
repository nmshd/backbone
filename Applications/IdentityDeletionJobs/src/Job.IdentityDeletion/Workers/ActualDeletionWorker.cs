using Backbone.BuildingBlocks.Application.Identities;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Devices.Application.Identities.Commands.HandleCompletedDeletionProcess;
using Backbone.Modules.Devices.Application.Identities.Commands.HandleErrorDuringIdentityDeletion;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity;
using Backbone.Modules.Devices.Application.Identities.Queries.ListAddressesOfIdentitiesWithDeletionProcessInStatusDeleting;
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
        // In case there was an error during a previous run, we need to make sure we also process those identities again.
        var addressesOfIdentitiesWithDeletionProcessesTriggeredInThePast = (await _mediator.Send(new ListAddressesOfIdentitiesWithDeletionProcessInStatusDeletingQuery(), cancellationToken)).Addresses;
        var addressesOfIdentitiesWithNewlyTriggeredDeletionProcesses = await TriggerRipeDeletionProcesses(cancellationToken);

        var allAddressesToProcess = addressesOfIdentitiesWithDeletionProcessesTriggeredInThePast.Union(addressesOfIdentitiesWithNewlyTriggeredDeletionProcesses).Distinct();

        await Delete(allAddressesToProcess);
    }

    private async Task<List<string>> TriggerRipeDeletionProcesses(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new TriggerRipeDeletionProcessesCommand(), cancellationToken);

        var addressesWithTriggeredDeletionProcesses = response.Results.Where(x => x.Value.IsSuccess).Select(x => x.Key).ToList();
        var erroringDeletionTriggers = response.Results.Where(x => x.Value.IsFailure);

        await NotifyIdentitiesAboutStartingDeletion(addressesWithTriggeredDeletionProcesses, cancellationToken);
        LogErroringDeletionTriggers(erroringDeletionTriggers);

        return addressesWithTriggeredDeletionProcesses;
    }

    private async Task NotifyIdentitiesAboutStartingDeletion(IEnumerable<string> addresses, CancellationToken cancellationToken)
    {
        foreach (var identityAddress in addresses)
        {
            await NotifyIdentityAboutStartingDeletion(identityAddress, cancellationToken);
        }
    }

    private async Task Delete(IEnumerable<string> addresses)
    {
        foreach (var identityAddress in addresses)
        {
            await TryDelete(identityAddress);
        }
    }

    private async Task NotifyIdentityAboutStartingDeletion(string identityAddress, CancellationToken cancellationToken)
    {
        await _pushNotificationSender.SendNotification(
            new DeletionStartsPushNotification(),
            SendPushNotificationFilter.AllDevicesOf(identityAddress),
            cancellationToken);
    }

    private async Task TryDelete(string identityAddress)
    {
        try
        {
            await Delete(identityAddress);
        }
        catch (Exception ex)
        {
            await _mediator.Send(new HandleErrorDuringIdentityDeletionCommand { IdentityAddress = identityAddress, ErrorMessage = ex.Message });
        }
    }

    private async Task Delete(string identityAddress)
    {
        var identity = await _mediator.Send(new GetIdentityQuery { Address = identityAddress });

        foreach (var identityDeleter in _identityDeleters)
        {
            try
            {
                await identityDeleter.Delete(identityAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute {identityDeleterName}.", identityDeleter);
                await _mediator.Send(new HandleErrorDuringIdentityDeletionCommand { IdentityAddress = identityAddress, ErrorMessage = ex.Message });

                // as soon as there is one error, we stop the deletion of this identity, because otherwise the deletion process will get deleted even though not all data has been deleted
                return;
            }
        }

        var usernames = identity.Devices.Select(d => d.Username);

        await _mediator.Send(new HandleCompletedDeletionProcessCommand { IdentityAddress = identityAddress, Usernames = usernames });
    }

    private void LogErroringDeletionTriggers(IEnumerable<KeyValuePair<string, UnitResult<DomainError>>> erroringDeletionTriggers)
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
