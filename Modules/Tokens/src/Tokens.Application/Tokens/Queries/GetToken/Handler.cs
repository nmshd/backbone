using AutoMapper;
using MediatR;
using Tokens.Application.Infrastructure;
using Tokens.Application.Tokens.DTOs;

namespace Tokens.Application.Tokens.Queries.GetToken;

public class Handler : IRequestHandler<GetTokenCommand, TokenDTO>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public Handler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TokenDTO> Handle(GetTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _unitOfWork.Tokens.Find(request.Id);
        return _mapper.Map<TokenDTO>(token);
    }
}
