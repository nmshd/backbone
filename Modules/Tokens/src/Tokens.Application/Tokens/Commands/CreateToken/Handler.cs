using AutoMapper;
using Backbone.Modules.Tokens.Application.Infrastructure;
using Backbone.Modules.Tokens.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;

public class Handler : IRequestHandler<CreateTokenCommand, CreateTokenResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public Handler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<CreateTokenResponse> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var newTokenEntity = new Token(_userContext.GetAddress(), _userContext.GetDeviceId(), request.Content, request.ExpiresAt);

        _unitOfWork.Tokens.Add(newTokenEntity);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<CreateTokenResponse>(newTokenEntity);
    }
}
