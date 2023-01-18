using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Tokens.Application.Infrastructure;

namespace Tokens.Application.Tokens.Commands;

public abstract class CommandHandlerBase<TRequest, TResponse> : RequestHandlerBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    protected readonly IUnitOfWork _unitOfWork;

    protected CommandHandlerBase(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper) : base(userContext, mapper)
    {
        _unitOfWork = unitOfWork;
    }

    public abstract override Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
