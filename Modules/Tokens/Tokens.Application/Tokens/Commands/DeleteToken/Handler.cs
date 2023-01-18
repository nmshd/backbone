using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.Extensions.Logging;
using Tokens.Application.Infrastructure;

namespace Tokens.Application.Tokens.Commands.DeleteToken;

public class Handler : CommandHandlerBase<DeleteTokenCommand, Unit>
{
    private readonly ILogger<Handler> _logger;

    public Handler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, ILogger<Handler> logger) : base(unitOfWork, userContext, mapper)
    {
        _logger = logger;
    }

    public override async Task<Unit> Handle(DeleteTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _unitOfWork.Tokens.Find(request.Id);

        if (token.CreatedBy != _activeIdentity)
            throw new ActionForbiddenException();

        _unitOfWork.Tokens.Remove(token);
        await _unitOfWork.SaveAsync();

        _logger.LogTrace($"Successfully deleted token with id {token.Id}.");

        return Unit.Value;
    }
}
