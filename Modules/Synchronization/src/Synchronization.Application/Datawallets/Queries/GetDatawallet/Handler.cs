using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Queries.GetDatawallet;

internal class Handler : IRequestHandler<GetDatawalletQuery, DatawalletDTO>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;
    private readonly IMapper _mapper;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<DatawalletDTO> Handle(GetDatawalletQuery request, CancellationToken cancellationToken)
    {
        var datawallet = await _dbContext.GetDatawallet(_activeIdentity, cancellationToken) ?? throw new NotFoundException(nameof(Datawallet));
        return _mapper.Map<DatawalletDTO>(datawallet);
    }
}
