﻿using System.Text.Json;
using AutoMapper;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;

public class Handler : IRequestHandler<FinalizeExternalEventSyncSyncRunCommand, FinalizeExternalEventSyncSyncRunResponse>,
    IRequestHandler<FinalizeDatawalletVersionUpgradeSyncRunCommand, FinalizeDatawalletVersionUpgradeSyncRunResponse>
{
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly IBlobStorage _blobStorage;
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private Datawallet _datawallet;
    private SyncRun _syncRun;
    private readonly BlobOptions _blobOptions;

    public Handler(ISynchronizationDbContext dbContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, IUserContext userContext, IMapper mapper, IEventBus eventBus)
    {
        _dbContext = dbContext;
        _blobStorage = blobStorage;
        _mapper = mapper;
        _eventBus = eventBus;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
        _blobOptions = blobOptions.Value;
    }

    public async Task<FinalizeDatawalletVersionUpgradeSyncRunResponse> Handle(FinalizeDatawalletVersionUpgradeSyncRunCommand request, CancellationToken cancellationToken)
    {
        _syncRun = await _dbContext.GetSyncRun(request.SyncRunId, _activeIdentity, cancellationToken);

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

        await _blobStorage.SaveAsync();
        await _dbContext.SaveChangesAsync(cancellationToken);

        PublishDatawalletModifiedIntegrationEvent();

        var response = new FinalizeDatawalletVersionUpgradeSyncRunResponse
        {
            NewDatawalletModificationIndex = _datawallet.LatestModification?.Index,
            DatawalletModifications = _mapper.Map<CreatedDatawalletModificationDTO[]>(newModifications)
        };

        PublishDatawalletModifiedIntegrationEvent();

        return response;
    }

    public async Task<FinalizeExternalEventSyncSyncRunResponse> Handle(FinalizeExternalEventSyncSyncRunCommand request, CancellationToken cancellationToken)
    {
        _syncRun = await _dbContext.GetSyncRunWithExternalEvents(request.SyncRunId, _activeIdentity, cancellationToken);

        if (_syncRun.Type != SyncRun.SyncRunType.ExternalEventSync)
            throw new ApplicationException(ApplicationErrors.SyncRuns.UnexpectedSyncRunType(SyncRun.SyncRunType.ExternalEventSync));

        CheckPreconditions();

        _datawallet = await _dbContext.GetDatawalletForInsertion(_activeIdentity, cancellationToken);

        if (_datawallet == null)
            throw new NotFoundException(nameof(Datawallet));

        var eventResults = _mapper.Map<ExternalEventResult[]>(request.ExternalEventResults);
        _syncRun.FinalizeExternalEventSync(eventResults);
        _dbContext.Set<SyncRun>().Update(_syncRun);

        var newModifications = AddModificationsToDatawallet(request.DatawalletModifications);
        _dbContext.Set<Datawallet>().Update(_datawallet);

        await _blobStorage.SaveAsync();
        await _dbContext.SaveChangesAsync(cancellationToken);

        PublishDatawalletModifiedIntegrationEvent();

        var response = new FinalizeExternalEventSyncSyncRunResponse
        {
            NewDatawalletModificationIndex = _datawallet.LatestModification?.Index,
            DatawalletModifications = _mapper.Map<CreatedDatawalletModificationDTO[]>(newModifications)
        };

        if (newModifications.Count > 0)
            PublishDatawalletModifiedIntegrationEvent();

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

        if (!modifications.Any())
            return new List<DatawalletModification>();

        var blobName = Guid.NewGuid().ToString("N");

        var newModifications = new List<DatawalletModification>();
        var payloads = new Dictionary<long, byte[]>();
        foreach (var modificationDto in modifications)
        {
            var newModification = _datawallet.AddModification(
                _mapper.Map<DatawalletModificationType>(modificationDto.Type),
                new Datawallet.DatawalletVersion(modificationDto.DatawalletVersion),
                modificationDto.Collection,
                modificationDto.ObjectIdentifier,
                modificationDto.PayloadCategory,
                modificationDto.EncryptedPayload,
                _activeDevice,
                blobName);

            newModifications.Add(newModification);

            if (newModification.EncryptedPayload != null)
                payloads.Add(newModification.Index, modificationDto.EncryptedPayload);
        }

        _blobStorage.Add(_blobOptions.RootFolder, blobName, JsonSerializer.SerializeToUtf8Bytes(payloads));

        return newModifications;
    }

    private void PublishDatawalletModifiedIntegrationEvent()
    {
        _eventBus.Publish(new DatawalletModifiedIntegrationEvent(_activeIdentity, _activeDevice));
    }
}
