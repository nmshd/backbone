using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using MediatR;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion;
public class Worker(IHostApplicationLifetime host, IEnumerable<IIdentityDeleter> identityDeleters, IMediator mediator) : IHostedService
{
    private readonly IHostApplicationLifetime _host = host;
    private readonly IEnumerable<IIdentityDeleter> _identityDeleters = identityDeleters;
    private readonly IMediator _mediator = mediator;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartProcessing(_mediator, _identityDeleters, cancellationToken);

        _host.StopApplication();
    }

    public static async Task StartProcessing(IMediator mediator, IEnumerable<IIdentityDeleter> identityDeleters, CancellationToken cancellationToken)
    {
        var identities = await mediator.Send(new UpdateDeletionProcessesCommand(), cancellationToken);

        foreach (var identityAddress in identities.IdentityAddresses)
        {
            foreach (var identityDeleter in identityDeleters)
            {
                await identityDeleter.Delete(identityAddress);
            }
            
            //await mediator.Send(new DeleteIdentityQuotasCommand(identityAddress), cancellationToken);
            //await mediator.Send(new DeleteIdentitySynchronizationCommand(identityAddress), cancellationToken);
            //await mediator.Send(new DeleteIdentityChallengesCommand(identityAddress), cancellationToken);
            //await mediator.Send(new DeleteIdentityFilesCommand(identityAddress), cancellationToken);
            //await mediator.Send(new DeleteIdentityMessagesCommand(identityAddress), cancellationToken);
            //await mediator.Send(new DeleteIdentityTokensCommand(identityAddress), cancellationToken);
            //await mediator.Send(new DeleteIdentityCommand(identityAddress), cancellationToken);


        }

        // for each of the identities
        // --- send notification
        // --- for all relationships
        // ---  --- create external event "PeerIdentityDeleted"
        // ---  --- (should delete relationships, their Changes, Template Allocations and Templates)
        // --- delete all challenges
        // --- 📍✅ delete all devices (in module, cascade on Devices.Identity deletion)
        // --- 📍✅ delete all users (in module, cascade on Devices.Device ↑ deletion)
        // --- delete all files
        // --- 📍 delete all Individual Quotas (in module)
        // --- 📍 delete all Tier Quotas (in module)
        // --- 📍 delete all MetricStatuses (in module)
        // --- delete all Datawallet and Datawallet modifications
        // --- delete all External Events
        // --- delete all Sync Runs
        // --- delete all Sync Errors
        // --- delete all Tokens
        // --- ✅ delete all Deletion Processes (cascade)
        // --- 📍 delete all PNS Registrations (in module)
        // --- modify messsages (replace old address and deviceID with dummies)
        // --- create Audit Log for identity deletion
        // --- Delete identity (with cascade for deletion processes)
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
