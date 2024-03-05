using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

public class Handler : IRequestHandler<ListMessagesQuery, ListMessagesResponse>
{
    private readonly IMapper _mapper;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, IMessagesRepository messagesRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _messagesRepository = messagesRepository;
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

        var response = new ListMessagesResponse(_mapper.Map<IEnumerable<MessageDTO>>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);

        response.PrepareForActiveIdentity(_userContext.GetAddress());

        return response;
    }
}
