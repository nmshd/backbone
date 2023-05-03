﻿using AutoMapper;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries;

public abstract class QueryHandlerBase<TRequest, TResponse> : RequestHandlerBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected readonly ITokenRepository _tokenRepository;

    protected QueryHandlerBase(ITokenRepository tokenRepository, IUserContext userContext, IMapper mapper) : base(userContext, mapper)
    {
        _tokenRepository = tokenRepository;
    }

    public abstract override Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
