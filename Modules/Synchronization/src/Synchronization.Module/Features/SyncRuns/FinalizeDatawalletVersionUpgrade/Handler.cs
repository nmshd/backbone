using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;
using MediatR;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeDatawalletVersionUpgrade;

public class Handler : SyncRunFinalization, IRequestHandler<Command, Response>
{
    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext) : base(dbContext, userContext)
    {
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        _syncRun = await _dbContext.GetSyncRun(SyncRunId.Parse(request.SyncRunId), _activeIdentity, cancellationToken);

        if (_syncRun.Type != SyncRun.SyncRunType.DatawalletVersionUpgrade)
            throw new ApplicationException(ApplicationErrors.SyncRuns.UnexpectedSyncRunType(SyncRun.SyncRunType.DatawalletVersionUpgrade));

        CheckPreconditions();

        _syncRun.FinalizeDatawalletVersionUpgrade();

        _datawallet = await _dbContext.GetDatawalletForInsertion(_activeIdentity, cancellationToken);

        if (_datawallet == null)
        {
            _datawallet = new Datawallet(new Datawallet.DatawalletVersion(request.NewDatawalletVersion), _activeIdentity);
            _dbContext.Set<Datawallet>().Add(_datawallet);
        }
        else
        {
            _datawallet.Upgrade(new Datawallet.DatawalletVersion(request.NewDatawalletVersion));
        }

        var newModifications = AddModificationsToDatawallet(request.DatawalletModifications);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new Response
        {
            NewDatawalletModificationIndex = _datawallet.LatestModification?.Index,
            DatawalletModifications = newModifications.Select(m => new CreatedDatawalletModificationDTO(m))
        };

        return response;
    }
}
