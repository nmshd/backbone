using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.GetDatawallet;

internal class Handler : IRequestHandler<Query, DatawalletDTO>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<DatawalletDTO> Handle(Query request, CancellationToken cancellationToken)
    {
        var datawallet = await _dbContext.GetDatawallet(_activeIdentity, cancellationToken) ?? throw new NotFoundException(nameof(Datawallet));
        return new DatawalletDTO(datawallet);
    }
}
