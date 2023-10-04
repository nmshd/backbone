using System.Text.Json;
using AutoMapper;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling.Extensions;
using Google.Apis.Logging;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetModifications;

public class Handler : IRequestHandler<GetModificationsQuery, GetModificationsResponse>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IBlobStorage _blobStorage;
    private readonly ILogger<Handler> _logger;
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly BlobOptions _blobOptions;

    public Handler(ISynchronizationDbContext dbContext, IMapper mapper, IUserContext userContext, IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, ILogger<Handler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _blobStorage = blobStorage;
        _logger = logger;
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

        var dtos = await MapToDtos(dbPaginationResult.ItemsOnPage);

        return new GetModificationsResponse(dtos, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }

    private async Task<List<DatawalletModificationDTO>> MapToDtos(IEnumerable<DatawalletModification> modifications)
    {
        var datawalletModifications = modifications as DatawalletModification[] ?? modifications.ToArray();

        var blobReferences = datawalletModifications.Where(m => !m.BlobReference.IsNullOrEmpty()).Select(m => m.BlobReference).Distinct();
        var blobs = await Task.WhenAll(blobReferences.Select(r =>
        {
            try
            {
                return _blobStorage.FindAsync(_blobOptions.RootFolder, r);
            }
            catch (NotFoundException)
            {
                throw new Exception($"Blob with reference '{r}' not found.");
            }
        }));

        var payloads = blobs
            .Select(b => JsonSerializer.Deserialize<Dictionary<long, byte[]>>(b))
            .SelectMany(b => b)
            .ToDictionary(b => b.Key, b => b.Value);

        var mappingTasks = datawalletModifications.Select(m => MapToDto(m, payloads));

        return (await Task.WhenAll(mappingTasks)).ToList();
    }

    private async Task<DatawalletModificationDTO> MapToDto(DatawalletModification modification, Dictionary<long, byte[]> payloads)
    {
        var dto = _mapper.Map<DatawalletModificationDTO>(modification);

        if (modification.BlobReference.IsNullOrEmpty())
        {
            try
            {
                dto.EncryptedPayload = await _blobStorage.FindAsync(_blobOptions.RootFolder, modification.Id);
            }
            catch (NotFoundException)
            {
                // blob not found means that there is no payload for this modification
            }
        }
        else
        {
            payloads.TryGetValue(modification.Index, out var payload);
            dto.EncryptedPayload = payload;
        }

        return dto;
    }
}
