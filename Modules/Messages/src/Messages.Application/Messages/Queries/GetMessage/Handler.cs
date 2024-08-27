using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Domain.Ids;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages.Queries.GetMessage;

public class Handler : IRequestHandler<GetMessageQuery, MessageDTO>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly IUserContext _userContext;
    private readonly ApplicationOptions _options;

    public Handler(IUserContext userContext, IMessagesRepository messagesRepository, IOptions<ApplicationOptions> options)
    {
        _userContext = userContext;
        _messagesRepository = messagesRepository;
        _options = options.Value;
    }

    public async Task<MessageDTO> Handle(GetMessageQuery request, CancellationToken cancellationToken)
    {
        var message = await _messagesRepository.Find(MessageId.Parse(request.Id), _userContext.GetAddress(), cancellationToken, true);

        var recipient = message.Recipients.FirstWithIdOrDefault(_userContext.GetAddress());

        recipient?.FetchedMessage(_userContext.GetDeviceId());

        await _messagesRepository.Update(message);

        var response = new MessageDTO(message, _userContext.GetAddress(), _options.DidDomainName);

        return response;
    }
}
