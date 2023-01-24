using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Synchronization.Application.Datawallets.DTOs;
using Synchronization.Application.Infrastructure;
using Synchronization.Domain.Entities;

namespace Synchronization.Application.Datawallets.Queries.GetDatawallet;

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
        var datawallet = await _dbContext.GetDatawallet(_activeIdentity, cancellationToken);

        if (datawallet == null)
            throw new NotFoundException(nameof(Datawallet));

        return _mapper.Map<DatawalletDTO>(datawallet);
    }
}
