using AutoMapper;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities;

public abstract class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected readonly IdentityAddress _activeIdentity;
    protected readonly IDevicesDbContext _dbContext;
    protected readonly IMapper _mapper;

    protected RequestHandlerBase(IDevicesDbContext dbContext, IUserContext userContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddressOrNull();
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
