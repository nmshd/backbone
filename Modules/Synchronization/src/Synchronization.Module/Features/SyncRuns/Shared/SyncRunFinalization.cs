using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;

public abstract class SyncRunFinalization
{
    protected readonly DeviceId _activeDevice;
    protected readonly IdentityAddress _activeIdentity;
    protected readonly ISynchronizationDbContext _dbContext;
    protected Datawallet? _datawallet;
    protected SyncRun _syncRun = null!;

    protected SyncRunFinalization(ISynchronizationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    protected void CheckPreconditions()
    {
        if (_syncRun.CreatedByDevice != _activeDevice)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotFinalizeSyncRunStartedByAnotherDevice());

        if (_syncRun.IsFinalized)
            throw new OperationFailedException(ApplicationErrors.SyncRuns.SyncRunAlreadyFinalized());
    }

    protected List<DatawalletModification> AddModificationsToDatawallet(List<PushDatawalletModificationItem> modifications)
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

    protected static DatawalletModificationType MapDatawalletModificationType(DatawalletModificationDTO.DatawalletModificationType type)
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
