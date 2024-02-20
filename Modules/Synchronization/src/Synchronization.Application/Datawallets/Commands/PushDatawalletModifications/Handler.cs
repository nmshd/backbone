using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Backbone.Modules.Synchronization.Domain.Entities.Datawallet;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

public class Handler : IRequestHandler<PushDatawalletModificationsCommand, PushDatawalletModificationsResponse>
{
    private readonly DeviceId _activeDevice;
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;

    private PushDatawalletModificationsCommand _request = null!;
    private CancellationToken _cancellationToken;
    private DatawalletVersion _supportedDatawalletVersion = null!;
    private Datawallet? _datawallet;
    private DatawalletModification[] _modifications = null!;
    private PushDatawalletModificationsResponse _response = null!;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext, IMapper mapper, IEventBus eventBus)
    {
        _dbContext = dbContext;
        _mapper = mapper;
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
        if (_supportedDatawalletVersion < _datawallet!.Version)
            throw new OperationFailedException(ApplicationErrors.Datawallet.InsufficientSupportedDatawalletVersion());
    }

    private async Task CreateModifications()
    {
        var blobName = Guid.NewGuid().ToString("N");

        var newModifications = _request.Modifications.Select(m => CreateModification(m, blobName));

        _dbContext.Set<Datawallet>().Update(_datawallet!);

        var modificationsArray = newModifications.ToArray();

        await Save(modificationsArray);

        _modifications = modificationsArray;
    }

    private DatawalletModification CreateModification(PushDatawalletModificationItem modificationDto, string blobReference)
    {
        return _datawallet!.AddModification(
            _mapper.Map<DatawalletModificationType>(modificationDto.Type),
            new DatawalletVersion(modificationDto.DatawalletVersion),
            modificationDto.Collection,
            modificationDto.ObjectIdentifier,
            modificationDto.PayloadCategory,
            modificationDto.EncryptedPayload,
            _activeDevice,
            blobReference
        );
    }

    private void EnsureDeviceIsUpToDate()
    {
        if (_datawallet!.LatestModification != null && _datawallet.LatestModification.Index != _request.LocalIndex)
            throw new OperationFailedException(ApplicationErrors.Datawallet.DatawalletNotUpToDate(_request.LocalIndex, _datawallet.LatestModification.Index));
    }

    private void BuildResponse()
    {
        var responseItems = _mapper.Map<PushDatawalletModificationsResponseItem[]>(_modifications);
        _response = new PushDatawalletModificationsResponse { Modifications = responseItems, NewIndex = responseItems.Max(i => i.Index) };
    }

    private async Task Save(DatawalletModification[] modifications)
    {
        await _dbContext.Set<DatawalletModification>().AddRangeAsync(modifications);

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
