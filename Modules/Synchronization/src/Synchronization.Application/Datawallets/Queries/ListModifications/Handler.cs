using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.ListModifications;

public class Handler : IRequestHandler<ListModificationsQuery, ListModificationsResponse>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<ListModificationsResponse> Handle(ListModificationsQuery request, CancellationToken cancellationToken)
    {
        var supportedDatawalletVersion = new Datawallet.DatawalletVersion(request.SupportedDatawalletVersion);

        var datawallet = await _dbContext.GetDatawallet(_activeIdentity, cancellationToken);

        if (supportedDatawalletVersion < (datawallet?.Version ?? 0))
            throw new OperationFailedException(ApplicationErrors.Datawallet.InsufficientSupportedDatawalletVersion());

        var dbPaginationResult = await _dbContext.GetDatawalletModifications(_activeIdentity, request.LocalIndex, request.PaginationFilter, cancellationToken);

        return new ListModificationsResponse(dbPaginationResult.ItemsOnPage.Select(modification => new DatawalletModificationDTO(modification)), request.PaginationFilter,
            dbPaginationResult.TotalNumberOfItems);
    }
}
