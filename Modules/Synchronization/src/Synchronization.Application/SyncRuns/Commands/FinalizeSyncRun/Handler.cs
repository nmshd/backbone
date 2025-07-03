using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class Handler : IRequestHandler<FinalizeExternalEventSyncSyncRunCommand, FinalizeExternalEventSyncSyncRunResponse>,
    IRequestHandler<FinalizeDatawalletVersionUpgradeSyncRunCommand, FinalizeDatawalletVersionUpgradeSyncRunResponse>
{
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;
    private Datawallet? _datawallet;
    private SyncRun _syncRun = null!;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<FinalizeDatawalletVersionUpgradeSyncRunResponse> Handle(FinalizeDatawalletVersionUpgradeSyncRunCommand request, CancellationToken cancellationToken)
    {
        _syncRun = await _dbContext.GetSyncRun(SyncRunId.Parse(request.SyncRunId), _activeIdentity, cancellationToken);

        if (_syncRun.Type != SyncRun.SyncRunType.DatawalletVersionUpgrade)
            throw new ApplicationException(ApplicationErrors.SyncRuns.UnexpectedSyncRunType(SyncRun.SyncRunType.DatawalletVersionUpgrade));

        CheckPreconditions();

        _syncRun.FinalizeDatawalletVersionUpgrade();
        _dbContext.Set<SyncRun>().Update(_syncRun);

        _datawallet = await _dbContext.GetDatawalletForInsertion(_activeIdentity, cancellationToken);

        if (_datawallet == null)
        {
            _datawallet = new Datawallet(new Datawallet.DatawalletVersion(request.NewDatawalletVersion), _activeIdentity);
            _dbContext.Set<Datawallet>().Add(_datawallet);
        }
        else
        {
            _datawallet.Upgrade(new Datawallet.DatawalletVersion(request.NewDatawalletVersion));
            _dbContext.Set<Datawallet>().Update(_datawallet);
        }

        var newModifications = AddModificationsToDatawallet(request.DatawalletModifications);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new FinalizeDatawalletVersionUpgradeSyncRunResponse
        {
            NewDatawalletModificationIndex = _datawallet.LatestModification?.Index,
            DatawalletModifications = newModifications.Select(m => new CreatedDatawalletModificationDTO(m))
        };

        return response;
    }

    public async Task<FinalizeExternalEventSyncSyncRunResponse> Handle(FinalizeExternalEventSyncSyncRunCommand request, CancellationToken cancellationToken)
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
        _dbContext.Set<SyncRun>().Update(_syncRun);

        var newModifications = AddModificationsToDatawallet(request.DatawalletModifications);
        _dbContext.Set<Datawallet>().Update(_datawallet);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var newUnsyncedExternalEventsExist = await _dbContext.DoNewUnsyncedExternalEventsExist(_activeIdentity, 0, cancellationToken);

        var response = new FinalizeExternalEventSyncSyncRunResponse
        {
            NewDatawalletModificationIndex = _datawallet.LatestModification?.Index,
            DatawalletModifications = newModifications.Select(x => new CreatedDatawalletModificationDTO(x)),
            NewUnsyncedExternalEventsExist = newUnsyncedExternalEventsExist
        };

        return response;
    }

    private void CheckPreconditions()
    {
        if (_syncRun.CreatedByDevice != _activeDevice)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotFinalizeSyncRunStartedByAnotherDevice());

        if (_syncRun.IsFinalized)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.SyncRunAlreadyFinalized());
    }

    private List<DatawalletModification> AddModificationsToDatawallet(List<PushDatawalletModificationItem> modifications)
    {
        if (_datawallet == null)
            throw new NotFoundException(nameof(Datawallet));

        if (modifications.Count == 0)
            return [];

        var newModifications = new List<DatawalletModification>();

        foreach (var modificationDto in modifications)
        {
            var newModification = _datawallet.AddModification(
                MapDatawalletModificationType(modificationDto.Type),
                new Datawallet.DatawalletVersion(modificationDto.DatawalletVersion),
                modificationDto.Collection,
                modificationDto.ObjectIdentifier,
                modificationDto.PayloadCategory,
                modificationDto.EncryptedPayload,
                _activeDevice);

            newModifications.Add(newModification);
        }

        return newModifications;
    }

    private static DatawalletModificationType MapDatawalletModificationType(DatawalletModificationDTO.DatawalletModificationType type)
    {
        return type switch
        {
            DatawalletModificationDTO.DatawalletModificationType.Create => DatawalletModificationType.Create,
            DatawalletModificationDTO.DatawalletModificationType.Update => DatawalletModificationType.Update,
            DatawalletModificationDTO.DatawalletModificationType.Delete => DatawalletModificationType.Delete,
            DatawalletModificationDTO.DatawalletModificationType.CacheChanged => DatawalletModificationType.CacheChanged,
            _ => throw new Exception($"Unsupported Datawallet Modification Type: {type}")
        };
    }
}
