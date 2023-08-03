using AutoMapper;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetModifications;

public class Handler : IRequestHandler<GetModificationsQuery, GetModificationsResponse>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IBlobStorage _blobStorage;
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly BlobOptions _blobOptions;

    public Handler(ISynchronizationDbContext dbContext, IMapper mapper, IUserContext userContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<GetModificationsResponse> Handle(GetModificationsQuery request, CancellationToken cancellationToken)
    {
        var supportedDatawalletVersion = new Datawallet.DatawalletVersion(request.SupportedDatawalletVersion);

        var datawallet = await _dbContext.GetDatawallet(_activeIdentity, cancellationToken);

        if (supportedDatawalletVersion < (datawallet?.Version ?? 0))
            throw new OperationFailedException(ApplicationErrors.Datawallet.InsufficientSupportedDatawalletVersion());

        var dbPaginationResult = await _dbContext.GetDatawalletModifications(_activeIdentity, request.LocalIndex, request.PaginationFilter, cancellationToken);

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
            datawalletModification.EncryptedPayload = await _blobStorage.FindAsync(_blobOptions.RootFolder, datawalletModification.Id);
        }
        catch (NotFoundException)
        {
            // if the payload was not found, it means that the modification had no payload
        }
    }
}
