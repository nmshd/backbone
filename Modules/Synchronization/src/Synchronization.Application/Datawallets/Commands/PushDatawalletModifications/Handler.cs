﻿using AutoMapper;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static Backbone.Modules.Synchronization.Domain.Entities.Datawallet;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

public class Handler : IRequestHandler<PushDatawalletModificationsCommand, PushDatawalletModificationsResponse>
{
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;

    private PushDatawalletModificationsCommand _request;
    private CancellationToken _cancellationToken;
    private DatawalletVersion _supportedDatawalletVersion;
    private Datawallet _datawallet;
    private DatawalletModification[] _modifications;
    private PushDatawalletModificationsResponse _response;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext, IMapper mapper, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, IEventBus eventBus)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
        _eventBus = eventBus;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<PushDatawalletModificationsResponse> Handle(PushDatawalletModificationsCommand request, CancellationToken cancellationToken)
    {
        _request = request;
        _cancellationToken = cancellationToken;
        _supportedDatawalletVersion = new DatawalletVersion(_request.SupportedDatawalletVersion);

        await EnsureNoActiveSyncRunExists(cancellationToken);
        await ReadDatawallet(cancellationToken);
        EnsureDatawalletExists();
        EnsureSufficientSupportedDatawalletVersion();
        EnsureDeviceIsUpToDate();
        await CreateModifications();
        PublishIntegrationEvent();
        BuildResponse();

        return _response;
    }

    private async Task ReadDatawallet(CancellationToken cancellationToken)
    {
        _datawallet = await _dbContext.GetDatawalletForInsertion(_activeIdentity, cancellationToken);
    }

    private void EnsureDatawalletExists()
    {
        if (_datawallet == null)
            throw new NotFoundException(nameof(Datawallet));
    }

    private async Task EnsureNoActiveSyncRunExists(CancellationToken cancellationToken)
    {
        var isActiveSyncRunAvailable = await _dbContext.IsActiveSyncRunAvailable(_activeIdentity, cancellationToken);

        if (isActiveSyncRunAvailable)
            throw new OperationFailedException(ApplicationErrors.Datawallet.CannotPushModificationsDuringActiveSyncRun());
    }

    private void EnsureSufficientSupportedDatawalletVersion()
    {
        if (_supportedDatawalletVersion < _datawallet.Version)
            throw new OperationFailedException(ApplicationErrors.Datawallet.InsufficientSupportedDatawalletVersion());
    }

    private async Task CreateModifications()
    {
        var newModifications = _request.Modifications.Select(CreateModification);

        _dbContext.Set<Datawallet>().Update(_datawallet);

        var modificationsArray = newModifications.ToArray();

        await Save(modificationsArray);

        _modifications = modificationsArray;
    }

    private DatawalletModification CreateModification(PushDatawalletModificationItem modificationDto)
    {
        return _datawallet.AddModification(
            _mapper.Map<DatawalletModificationType>(modificationDto.Type),
            new DatawalletVersion(modificationDto.DatawalletVersion),
            modificationDto.Collection,
            modificationDto.ObjectIdentifier,
            modificationDto.PayloadCategory,
            modificationDto.EncryptedPayload,
            _activeDevice
        );
    }

    private void EnsureDeviceIsUpToDate()
    {
        if (_datawallet.LatestModification != null && _datawallet.LatestModification.Index != _request.LocalIndex)
            throw new OperationFailedException(ApplicationErrors.Datawallet.DatawalletNotUpToDate(_request.LocalIndex, _datawallet.LatestModification.Index));
    }

    private void BuildResponse()
    {
        var responseItems = _mapper.Map<PushDatawalletModificationsResponseItem[]>(_modifications);
        _response = new PushDatawalletModificationsResponse { Modifications = responseItems, NewIndex = responseItems.Max(i => i.Index) };
    }

    private async Task Save(DatawalletModification[] modifications)
    {
        await _dbContext.Set<DatawalletModification>().AddRangeAsync(modifications, _cancellationToken);
        foreach (var newModification in modifications)
        {
            if (newModification.EncryptedPayload != null)
                _blobStorage.Add(_blobOptions.RootFolder, newModification.Id, newModification.EncryptedPayload);
        }

        await _blobStorage.SaveAsync();

        try
        {
            await _dbContext.SaveChangesAsync(_cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            if (ex.HasReason(DbUpdateExceptionReason.DuplicateIndex))
                throw new OperationFailedException(ApplicationErrors.Datawallet.DatawalletNotUpToDate());

            throw;
        }
    }

    private void PublishIntegrationEvent()
    {
        _eventBus.Publish(new DatawalletModifiedIntegrationEvent(_activeIdentity, _activeDevice));
    }
}
