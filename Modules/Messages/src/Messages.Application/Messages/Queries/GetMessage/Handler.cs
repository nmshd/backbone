using AutoMapper;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.GetMessage;

public class Handler : IRequestHandler<GetMessageQuery, MessageDTO>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly IMapper _mapper;
    private readonly MessageService _messageService;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, MessageService messageService, IMessagesRepository messagesRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _messageService = messageService;
        _messagesRepository = messagesRepository;
    }

    public async Task<MessageDTO> Handle(GetMessageQuery request, CancellationToken cancellationToken)
    {
        var message = await _messagesRepository.Find(request.Id, _userContext.GetAddress(), cancellationToken);
        
        await _messageService.MarkMessageAsReceived(message);

        var response = _mapper.Map<MessageDTO>(message);

        if (!request.NoBody)
            await _messageService.FillBody(response);

        response.PrepareForActiveIdentity(_userContext.GetAddress());

        return response;
    }
}
