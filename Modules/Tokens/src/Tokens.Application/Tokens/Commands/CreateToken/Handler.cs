using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class Handler : IRequestHandler<CreateTokenCommand, CreateTokenResponse>
{
    private readonly IMapper _mapper;
    private readonly ITokensRepository _tokensRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, ITokensRepository tokensRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _tokensRepository = tokensRepository;
    }

    public async Task<CreateTokenResponse> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var forIdentity = request.ForIdentity == null ? null : IdentityAddress.Parse(request.ForIdentity);
        var newTokenEntity = new Token(_userContext.GetAddress(), _userContext.GetDeviceId(), request.Content, request.ExpiresAt, forIdentity);

        await _tokensRepository.Add(newTokenEntity);

        return _mapper.Map<CreateTokenResponse>(newTokenEntity);
    }
}
