using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
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
    private CancellationToken _cancellationToken;
    private Datawallet? _datawallet;
    private DatawalletModification[] _modifications = null!;

    private PushDatawalletModificationsCommand _request = null!;
    private PushDatawalletModificationsResponse _response = null!;
    private DatawalletVersion _supportedDatawalletVersion = null!;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
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
        var newModifications = _request.Modifications.Select(CreateModification);

        _dbContext.Set<Datawallet>().Entry(_datawallet!).CurrentValues.SetValues(_datawallet!);

        var modificationsArray = newModifications.ToArray();

        await Save(modificationsArray);

        _modifications = modificationsArray;
    }

    private DatawalletModification CreateModification(PushDatawalletModificationItem modificationDto)
    {
        return _datawallet!.AddModification(
            MapDatawalletModificationType(modificationDto.Type),
            new DatawalletVersion(modificationDto.DatawalletVersion),
            modificationDto.Collection,
            modificationDto.ObjectIdentifier,
            modificationDto.PayloadCategory,
            modificationDto.EncryptedPayload,
            _activeDevice
        );
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

    private void EnsureDeviceIsUpToDate()
    {
        if (_datawallet!.LatestModification != null && _datawallet.LatestModification.Index != _request.LocalIndex)
            throw new OperationFailedException(ApplicationErrors.Datawallet.DatawalletNotUpToDate(_request.LocalIndex, _datawallet.LatestModification.Index));
    }

    private void BuildResponse()
    {
        var responseItems = _modifications.Select(m => new PushDatawalletModificationsResponseItem(m));
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
}
