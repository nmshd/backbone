﻿using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Backbone.Modules.Synchronization.Domain.Entities.Datawallet;

namespace Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;

public class Handler : IRequestHandler<StartSyncRunCommand, StartSyncRunResponse>
{
    private const int MAX_NUMBER_OF_SYNC_ERRORS_PER_EVENT = 3;
    private const int DEFAULT_DURATION = 10;
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;

    private readonly ISynchronizationDbContext _dbContext;
    private readonly IMapper _mapper;

    private CancellationToken _cancellationToken;
    private Datawallet _datawallet;
    private SyncRun _previousSyncRun;
    private StartSyncRunCommand _request;
    private DatawalletVersion _supportedDatawalletVersion;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }


    public async Task<StartSyncRunResponse> Handle(StartSyncRunCommand request, CancellationToken cancellationToken)
    {
        _request = request;
        _supportedDatawalletVersion = new DatawalletVersion(_request.SupportedDatawalletVersion);
        _cancellationToken = cancellationToken;
        _datawallet = await _dbContext.GetDatawallet(_activeIdentity, _cancellationToken);

        return request.Type switch
        {
            SyncRunDTO.SyncRunType.DatawalletVersionUpgrade => await StartDatawalletVersionUpgrade(),
            SyncRunDTO.SyncRunType.ExternalEventSync => await StartExternalEventSync(),
            _ => throw new Exception("Given sync run type is not supported.")
        };
    }

    private async Task<StartSyncRunResponse> StartExternalEventSync()
    {
        EnsureDatawalletExists();
        EnsureSufficientSupportedDatawalletVersion();
        await EnsureNoActiveSyncRunExists();

        var unsyncedEvents = await _dbContext.GetUnsyncedExternalEvents(_activeIdentity, MAX_NUMBER_OF_SYNC_ERRORS_PER_EVENT, _cancellationToken);

        if (unsyncedEvents.Count == 0)
            return CreateResponse(StartSyncRunStatus.NoNewEvents);

        var newSyncRun = await CreateNewSyncRun(unsyncedEvents);

        return CreateResponse(StartSyncRunStatus.Created, newSyncRun);
    }

    private async Task<StartSyncRunResponse> StartDatawalletVersionUpgrade()
    {
        EnsureSufficientSupportedDatawalletVersion();
        await EnsureNoActiveSyncRunExists();

        var newSyncRun = await CreateNewSyncRun();

        return CreateResponse(StartSyncRunStatus.Created, newSyncRun);
    }

    private void EnsureDatawalletExists()
    {
        if (_datawallet == null)
            throw new NotFoundException(nameof(Datawallet));
    }

    private void EnsureSufficientSupportedDatawalletVersion()
    {
        if (_datawallet != null && _supportedDatawalletVersion < _datawallet.Version)
            throw new OperationFailedException(ApplicationErrors.Datawallet.InsufficientSupportedDatawalletVersion());
    }

    private async Task EnsureNoActiveSyncRunExists()
    {
        _previousSyncRun = await _dbContext.GetPreviousSyncRunWithExternalEvents(_activeIdentity, _cancellationToken);

        if (IsPreviousSyncRunStillActive())
        {
            if (!_previousSyncRun.IsExpired)
                throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotStartSyncRunWhenAnotherSyncRunIsRunning(_previousSyncRun.Id));

            await CancelPreviousSyncRun();
        }
    }

    private bool IsPreviousSyncRunStillActive()
    {
        return _previousSyncRun is { IsFinalized: false };
    }

    private async Task CancelPreviousSyncRun()
    {
        _previousSyncRun.Cancel();
        _dbContext.Set<SyncRun>().Update(_previousSyncRun);
        await _dbContext.SaveChangesAsync(_cancellationToken);
    }

    private async Task<SyncRun> CreateNewSyncRun()
    {
        return await CreateNewSyncRun(Array.Empty<ExternalEvent>());
    }

    private async Task<SyncRun> CreateNewSyncRun(IEnumerable<ExternalEvent> events)
    {
        var newIndex = DetermineNextSyncRunIndex();
        var syncRun = new SyncRun(newIndex, _request.Duration ?? DEFAULT_DURATION, _activeIdentity, _activeDevice, events ?? Array.Empty<ExternalEvent>(), _mapper.Map<SyncRun.SyncRunType>(_request.Type));

        await _dbContext.Set<SyncRun>().AddAsync(syncRun, _cancellationToken);
        try
        {
            await _dbContext.SaveChangesAsync(_cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            if (ex.HasReason(DbUpdateExceptionReason.DuplicateIndex) || ex.HasReason(DbUpdateExceptionReason.UniqueKeyViolation))
                throw new OperationFailedException(ApplicationErrors.SyncRuns.CannotStartSyncRunWhenAnotherSyncRunIsRunning());

            throw;
        }

        return syncRun;
    }

    private long DetermineNextSyncRunIndex()
    {
        if (_previousSyncRun == null)
            return 0;

        return _previousSyncRun.Index + 1;
    }

    private StartSyncRunResponse CreateResponse(StartSyncRunStatus status, SyncRun newSyncRun = null)
    {
        var syncRunDTO = _mapper.Map<SyncRunDTO>(newSyncRun);

        var response = new StartSyncRunResponse
        {
            Status = status,
            SyncRun = syncRunDTO
        };
        return response;
    }
}
