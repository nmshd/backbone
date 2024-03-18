using AutoMapper;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class Handler : IRequestHandler<GetTokenQuery, TokenDTO>
{
    private readonly IMapper _mapper;
    private readonly ITokensRepository _tokensRepository;

    public Handler(IMapper mapper, ITokensRepository tokensRepository)
    {
        _mapper = mapper;
        _tokensRepository = tokensRepository;
    }

    public async Task<TokenDTO> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        var token = await _tokensRepository.Find(request.Id);
        return _mapper.Map<TokenDTO>(token);
    }
}
