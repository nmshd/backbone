using Backbone.BuildingBlocks.Application.Identities;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using MediatR;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion;
public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IEnumerable<IIdentityDeleter> _identityDeleters;

    public Worker(IHostApplicationLifetime host, IServiceScopeFactory serviceScopeFactory, IEnumerable<IIdentityDeleter> identityDeleters)
    {
        _host = host;
        _serviceScopeFactory = serviceScopeFactory;
        _identityDeleters = identityDeleters;
        using var scope = _serviceScopeFactory.CreateScope();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await StartProcessing(mediator, cancellationToken);

        _host.StopApplication();
    }

    internal async Task StartProcessing(IMediator mediator, CancellationToken cancellationToken)
    {
        var identities = await mediator.Send(new UpdateDeletionProcessesCommand(), cancellationToken);

        foreach (var identityAddress in identities.IdentityAddresses)
        {
            foreach (var identityDeleter in _identityDeleters)
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
