using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages;

public abstract class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : notnull
{
    protected readonly IdentityAddress _activeIdentity;
    protected readonly IEventBus _eventBus;
    protected readonly IMapper _mapper;

    protected RequestHandlerBase(IUserContext userContext, IMapper mapper, IEventBus eventBus)
    {
        _mapper = mapper;
        _eventBus = eventBus;
        _activeIdentity = userContext.GetAddress();
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
