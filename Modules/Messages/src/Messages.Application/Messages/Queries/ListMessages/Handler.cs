﻿using AutoMapper;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

public class Handler : IRequestHandler<ListMessagesQuery, ListMessagesResponse>
{
    private readonly IMapper _mapper;
    private readonly MessageService _messageService;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IUserContext _userContext;

    public Handler(IUserContext userContext, IMapper mapper, MessageService messageService, IMessagesRepository messagesRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _messageService = messageService;
        _messagesRepository = messagesRepository;
    }

    public async Task<ListMessagesResponse> Handle(ListMessagesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await _messagesRepository.FindMessagesOfIdentity(_userContext.GetAddress(), request, cancellationToken);

        var response = new ListMessagesResponse(_mapper.Map<IEnumerable<MessageDTO>>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);

        await _messageService.FillBodies(response);

        await _messageService.MarkMessagesAsReceived(dbPaginationResult.ItemsOnPage, cancellationToken);

        response.PrepareForActiveIdentity(_userContext.GetAddress());

        return response;
    }
}
