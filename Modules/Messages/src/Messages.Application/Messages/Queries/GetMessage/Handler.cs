using AutoMapper;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.GetMessage;

public class Handler : IRequestHandler<GetMessageQuery, MessageDTO>
{
    private readonly IMessagesDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly MessageService _messageService;
    private readonly IUserContext _userContext;

    public Handler(IMessagesDbContext dbContext, IUserContext userContext, IMapper mapper, MessageService messageService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _mapper = mapper;
        _messageService = messageService;
    }

    public async Task<MessageDTO> Handle(GetMessageQuery request, CancellationToken cancellationToken)
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
