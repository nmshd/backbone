using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Synchronization.Application.Datawallets.DTOs;
using Synchronization.Domain.Entities;

namespace Synchronization.Application.Datawallets.Queries.GetModifications;

public class Handler : IRequestHandler<GetModificationsQuery, GetModificationsResponse>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IBlobStorage _blobStorage;
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IMapper _mapper;

    public Handler(ISynchronizationDbContext dbContext, IMapper mapper, IUserContext userContext, IBlobStorage blobStorage)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _blobStorage = blobStorage;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<GetModificationsResponse> Handle(GetModificationsQuery request, CancellationToken cancellationToken)
    {
        var supportedDatawalletVersion = new Datawallet.DatawalletVersion(request.SupportedDatawalletVersion);

        var datawallet = await _dbContext.GetDatawallet(_activeIdentity, cancellationToken);

        if (supportedDatawalletVersion < (datawallet?.Version ?? 0))
            throw new OperationFailedException(ApplicationErrors.Datawallet.InsufficientSupportedDatawalletVersion());

        var dbPaginationResult = await _dbContext.GetDatawalletModifications(_activeIdentity, request.LocalIndex, request.PaginationFilter);

        var dtos = _mapper.Map<IEnumerable<DatawalletModificationDTO>>(dbPaginationResult.ItemsOnPage).ToArray();

        await FillEncryptedPayloads(dtos);

        return new GetModificationsResponse(dtos, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }

    private async Task FillEncryptedPayloads(IEnumerable<DatawalletModificationDTO> modifications)
    {
        await Task.WhenAll(modifications.Select(FillEncryptedPayload));
    }

    private async Task FillEncryptedPayload(DatawalletModificationDTO datawalletModification)
    {
        try
        {
            datawalletModification.EncryptedPayload = await _blobStorage.FindAsync(datawalletModification.Id);
        }
        catch (NotFoundException)
        {
            // if the payload was not found, it means that the modification had no payload
        }
    }
}
