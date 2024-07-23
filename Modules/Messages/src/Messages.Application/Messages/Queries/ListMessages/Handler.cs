using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

public class Handler : IRequestHandler<ListMessagesQuery, ListMessagesResponse>
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

    public async Task<ListMessagesResponse> Handle(ListMessagesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _messagesRepository.FindMessagesWithIds(request.Ids, _userContext.GetAddress(), request.PaginationFilter, cancellationToken, track: true);

        foreach (var message in dbPaginationResult.ItemsOnPage)
        {
            var recipient = message.Recipients.FirstWithIdOrDefault(_userContext.GetAddress());

            recipient?.FetchedMessage(_userContext.GetDeviceId());
        }

        await _messagesRepository.Update(dbPaginationResult.ItemsOnPage);

        var response = new ListMessagesResponse(dbPaginationResult.ItemsOnPage, request.PaginationFilter, dbPaginationResult.TotalNumberOfItems, _userContext.GetAddress(), _options.DidDomainName);

        return response;
    }
}
