using AutoMapper;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class Handler : IRequestHandler<GetTokenQuery, TokenDTO>
{
    private readonly IMapper _mapper;
    private readonly ITokenRepository _tokenRepository;

    public Handler(IMapper mapper, ITokenRepository tokenRepository)
    {
        _mapper = mapper;
        _tokenRepository = tokenRepository;
    }

    public async Task<TokenDTO> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        var token = await _tokenRepository.Find(request.Id);
        return _mapper.Map<TokenDTO>(token);
    }
}
