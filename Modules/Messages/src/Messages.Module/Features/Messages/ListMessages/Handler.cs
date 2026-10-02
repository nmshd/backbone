using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Messages.Abstractions.Persistence;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Modules.Messages.Module.Features.Messages.Shared;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Module.Features.Messages.ListMessages;

public class Handler : IRequestHandler<Query, Response>
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

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var result = await _messagesRepository.ListMessagesWithContent(request.Ids.Select(MessageId.Parse), _userContext.GetAddress(),
            request.PaginationFilter, cancellationToken, track: true);

        foreach (var message in result.ItemsOnPage)
            message.Recipients.FirstWithIdOrDefault(_userContext.GetAddress())?.FetchedMessage(_userContext.GetDeviceId());

        await _messagesRepository.Update(result.ItemsOnPage);
        return new Response(result, request.PaginationFilter, _userContext.GetAddress(), _configuration.DidDomainName);
    }
}
