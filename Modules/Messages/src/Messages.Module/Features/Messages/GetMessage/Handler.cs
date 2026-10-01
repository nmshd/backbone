using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Messages.Abstractions.Persistence;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Module.Features.Messages.Shared;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Module.Features.Messages.GetMessage;

public class Handler : IRequestHandler<GetMessageQuery, MessageDTO>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly IUserContext _userContext;
    private readonly ApplicationConfiguration _configuration;

    public Handler(IUserContext userContext, IMessagesRepository messagesRepository, IOptions<ApplicationConfiguration> options)
    {
        _userContext = userContext;
        _messagesRepository = messagesRepository;
        _configuration = options.Value;
    }

    public async Task<MessageDTO> Handle(GetMessageQuery request, CancellationToken cancellationToken)
    {
        var message = await _messagesRepository.GetWithContent(MessageId.Parse(request.Id), _userContext.GetAddress(), cancellationToken, true);
        message.Recipients.FirstWithIdOrDefault(_userContext.GetAddress())?.FetchedMessage(_userContext.GetDeviceId());
        await _messagesRepository.Update(message);
        return new MessageDTO(message, _userContext.GetAddress(), _configuration.DidDomainName);
    }
}
