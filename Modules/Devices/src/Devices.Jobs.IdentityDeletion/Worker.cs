﻿using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using MediatR;

namespace Backbone.Modules.Devices.Jobs.IdentityDeletion;
public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime _host;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Worker(IHostApplicationLifetime host, IServiceScopeFactory serviceScopeFactory)
    {
        _host = host;
        _serviceScopeFactory = serviceScopeFactory;
        using var scope = _serviceScopeFactory.CreateScope();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await StartProcessing(mediator, cancellationToken);

        _host.StopApplication();
    }

    internal static async Task StartProcessing(IMediator mediator, CancellationToken cancellationToken)
    {
        var identityAddresses = await mediator.Send(new UpdateDeletionProcessesCommand(), cancellationToken);

        // for each of the identities
        // --- send notification
        // --- for all relationships
        // ---  --- create external event "PeerIdentityDeleted"
        // ---  --- (should delete relationships, their Changes, Template Allocations and Templates)
        // --- delete all challenges
        // --- delete all devices
        // --- delete all users
        // --- delete all files
        // --- delete all Individual Quotas
        // --- delete all Tier Quotas
        // --- delete all MetricStatuses
        // --- delete all Datawallet and Datawallet modifications
        // --- delete all External Events
        // --- delete all Sync Runs
        // --- delete all Sync Errors
        // --- delete all Tokens
        // --- ✅ delete all Deletion Processes (cascade)
        // --- delete all PNS Registrations
        // --- modify messsages (replace old address and deviceID with dummies)
        // --- create Audit Log for identity deletion
        // --- Delete identity (with cascade for deletion processes)
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
