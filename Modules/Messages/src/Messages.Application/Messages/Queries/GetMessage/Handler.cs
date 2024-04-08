using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.GetMessage;

public class Handler : IRequestHandler<GetMessageQuery, MessageDTO>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, IMessagesRepository messagesRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _messagesRepository = messagesRepository;
    }

    public async Task<MessageDTO> Handle(GetMessageQuery request, CancellationToken cancellationToken)
    {
        var message = await _messagesRepository.Find(request.Id, _userContext.GetAddress(), cancellationToken, track: true);

        var recipient = message.Recipients.FirstWithIdOrDefault(_userContext.GetAddress());

        recipient?.FetchedMessage(_userContext.GetDeviceId());

        await _messagesRepository.Update(message);

        var response = _mapper.Map<MessageDTO>(message);

        response.PrepareForActiveIdentity(_userContext.GetAddress());

        return response;
    }
}
