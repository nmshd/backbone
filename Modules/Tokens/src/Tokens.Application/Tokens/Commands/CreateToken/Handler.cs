using AutoMapper;
using Backbone.Modules.Tokens.Application.Infrastructure;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class Handler : IRequestHandler<CreateTokenCommand, CreateTokenResponse>
{
    private readonly IMapper _mapper;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, ITokenRepository tokenRepository)
    {
        
        _userContext = userContext;
        _mapper = mapper;
        _tokenRepository = tokenRepository;
    }

    public async Task<CreateTokenResponse> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var newTokenEntity = new Token(_userContext.GetAddress(), _userContext.GetDeviceId(), request.Content, request.ExpiresAt);

        await _tokenRepository.Add(newTokenEntity);

        return _mapper.Map<CreateTokenResponse>(newTokenEntity);
    }
}
