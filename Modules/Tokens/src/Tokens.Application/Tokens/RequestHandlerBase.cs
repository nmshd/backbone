using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens;

public abstract class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected readonly IdentityAddress _activeIdentity;
    protected readonly IMapper _mapper;

    protected RequestHandlerBase(IUserContext userContext, IMapper mapper)
    {
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
