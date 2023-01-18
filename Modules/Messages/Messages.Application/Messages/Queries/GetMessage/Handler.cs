using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Messages.Application.Extensions;
using Messages.Application.Messages.DTOs;
using Messages.Domain.Entities;

namespace Messages.Application.Messages.Queries.GetMessage;

public class Handler : IRequestHandler<GetMessageCommand, MessageDTO>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly MessageService _messageService;
    private readonly IUserContext _userContext;

    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper, MessageService messageService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _mapper = mapper;
        _messageService = messageService;
    }

    public async Task<MessageDTO> Handle(GetMessageCommand request, CancellationToken cancellationToken)
    {
        var addressOfActiveIdentity = _userContext.GetAddress();

        var message = await _dbContext
            .Set<Message>()
            .IncludeAllReferences(addressOfActiveIdentity)
            .WithSenderOrRecipient(_userContext.GetAddress())
            .FirstWithId(request.Id, cancellationToken);

        await _messageService.MarkMessageAsReceived(message, cancellationToken);

        var response = _mapper.Map<MessageDTO>(message);

        if (!request.NoBody)
            await _messageService.FillBody(response);

        response.PrepareForActiveIdentity(_userContext.GetAddress());

        return response;
    }
}
