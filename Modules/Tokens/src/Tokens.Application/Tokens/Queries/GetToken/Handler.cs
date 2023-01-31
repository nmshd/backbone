using AutoMapper;
using Backbone.Modules.Tokens.Application.Infrastructure;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;

public class Handler : IRequestHandler<GetTokenQuery, TokenDTO>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public Handler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TokenDTO> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        var token = await _unitOfWork.Tokens.Find(request.Id);
        return _mapper.Map<TokenDTO>(token);
    }
}
