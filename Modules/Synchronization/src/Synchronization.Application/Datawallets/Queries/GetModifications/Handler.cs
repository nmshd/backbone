using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetModifications;

public class Handler : IRequestHandler<GetModificationsQuery, GetModificationsResponse>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IMapper _mapper;

    public Handler(ISynchronizationDbContext dbContext, IMapper mapper, IUserContext userContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<GetModificationsResponse> Handle(GetModificationsQuery request, CancellationToken cancellationToken)
    {
        var supportedDatawalletVersion = new Datawallet.DatawalletVersion(request.SupportedDatawalletVersion);

        var datawallet = await _dbContext.GetDatawallet(_activeIdentity, cancellationToken);

        if (supportedDatawalletVersion < (datawallet?.Version ?? 0))
            throw new OperationFailedException(ApplicationErrors.Datawallet.InsufficientSupportedDatawalletVersion());

        var dbPaginationResult = await _dbContext.GetDatawalletModifications(_activeIdentity, request.LocalIndex, request.PaginationFilter, cancellationToken);

        var dtos = MapToDtos(dbPaginationResult.ItemsOnPage);

        return new GetModificationsResponse(dtos, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);
    }

    private List<DatawalletModificationDTO> MapToDtos(IEnumerable<DatawalletModification> modifications)
    {
        var datawalletModifications = modifications as DatawalletModification[] ?? modifications.ToArray();

        var mappingTasks = datawalletModifications.Select(MapToDto);

        return mappingTasks.ToList();
    }

    private DatawalletModificationDTO MapToDto(DatawalletModification modification)
    {
        var dto = _mapper.Map<DatawalletModificationDTO>(modification);
        return dto;
    }
}
