using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;
using MediatR;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeExternalEventSync;

public class Handler : SyncRunFinalization, IRequestHandler<Command, Response>
{
    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext) : base(dbContext, userContext)
    {
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        _syncRun = await _dbContext.GetSyncRunWithExternalEvents(SyncRunId.Parse(request.SyncRunId), _activeIdentity, cancellationToken);

        if (_syncRun.Type != SyncRun.SyncRunType.ExternalEventSync)
            throw new ApplicationException(ApplicationErrors.SyncRuns.UnexpectedSyncRunType(SyncRun.SyncRunType.ExternalEventSync));

        CheckPreconditions();

        _datawallet = await _dbContext.GetDatawalletForInsertion(_activeIdentity, cancellationToken) ?? throw new NotFoundException(nameof(Datawallet));

        var eventResults = request.ExternalEventResults.Select(e =>
            new ExternalEventResult
            {
                ErrorCode = e.ErrorCode ?? string.Empty,
                ExternalEventId = ExternalEventId.Parse(e.ExternalEventId)
            }).ToArray();

        _syncRun.FinalizeExternalEventSync(eventResults);

        var newModifications = AddModificationsToDatawallet(request.DatawalletModifications);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var newUnsyncedExternalEventsExist = await _dbContext.DoNewUnsyncedExternalEventsExist(_activeIdentity, 1, cancellationToken);

        var response = new Response
        {
            NewDatawalletModificationIndex = _datawallet.LatestModification?.Index,
            DatawalletModifications = newModifications.Select(x => new CreatedDatawalletModificationDTO(x)),
            NewUnsyncedExternalEventsExist = newUnsyncedExternalEventsExist
        };

        return response;
    }
}
