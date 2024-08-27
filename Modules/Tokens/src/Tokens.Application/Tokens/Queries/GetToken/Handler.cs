using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class Handler : IRequestHandler<GetTokenQuery, TokenDTO>
{
    private readonly IMapper _mapper;
    private readonly ITokensRepository _tokensRepository;
    private readonly IUserContext _userContext;

    public Handler(IMapper mapper, ITokensRepository tokensRepository, IUserContext userContext)
    {
        _mapper = mapper;
        _tokensRepository = tokensRepository;
        _userContext = userContext;
    }

    public async Task<TokenDTO> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        var token = await _tokensRepository.Find(request.Id, _userContext.GetAddressOrNull()) ?? throw new NotFoundException();
        return _mapper.Map<TokenDTO>(token);
    }
}
