using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Tokens.Application.Infrastructure;

namespace Tokens.Application.Tokens.Queries;

public abstract class QueryHandlerBase<TRequest, TResponse> : RequestHandlerBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected readonly ITokenRepository _tokenRepository;

    protected QueryHandlerBase(ITokenRepository tokenRepository, IUserContext userContext, IMapper mapper) : base(userContext, mapper)
    {
        _tokenRepository = tokenRepository;
    }

    public abstract override Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
