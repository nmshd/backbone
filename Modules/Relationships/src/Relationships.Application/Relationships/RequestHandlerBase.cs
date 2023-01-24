using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Relationships.Application.Infrastructure.Persistence;

namespace Relationships.Application.Relationships;

public abstract class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected readonly IdentityAddress _activeIdentity;
    protected readonly DeviceId _activeDevice;
    protected readonly IRelationshipsDbContext _dbContext;
    protected readonly IMapper _mapper;

    protected RequestHandlerBase(IRelationshipsDbContext dbContext, IUserContext userContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
        _activeDevice = userContext.GetDeviceId();
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
