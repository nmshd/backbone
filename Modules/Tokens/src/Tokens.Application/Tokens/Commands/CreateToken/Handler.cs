using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.IntegrationEvents;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class Handler : IRequestHandler<CreateTokenCommand, CreateTokenResponse>
{
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly ITokensRepository _tokensRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, IEventBus eventBus, ITokensRepository tokensRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _tokensRepository = tokensRepository;
        _eventBus = eventBus;
    }

    public async Task<CreateTokenResponse> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var newTokenEntity = new Token(_userContext.GetAddress(), _userContext.GetDeviceId(), request.Content, request.ExpiresAt);

        await _tokensRepository.Add(newTokenEntity);

        PublishIntegrationEvent(newTokenEntity);

        return _mapper.Map<CreateTokenResponse>(newTokenEntity);
    }

    private void PublishIntegrationEvent(Token newToken)
    {
        var evt = new TokenCreatedIntegrationEvent(newToken);
        _eventBus.Publish(evt);
    }
}
