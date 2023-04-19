using AutoMapper;
using Backbone.Modules.Messages.Application.Extensions;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

public class Handler : IRequestHandler<ListMessagesQuery, ListMessagesResponse>
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

    public async Task<ListMessagesResponse> Handle(ListMessagesQuery request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await FindMessagesOfIdentity(_userContext.GetAddress(), request);

        var response = new ListMessagesResponse(_mapper.Map<IEnumerable<MessageDTO>>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);

        await _messageService.FillBodies(response);

        await _messageService.MarkMessagesAsReceived(dbPaginationResult.ItemsOnPage, cancellationToken);

        response.PrepareForActiveIdentity(_userContext.GetAddress());

        return response;
    }

    private async Task<DbPaginationResult<Message>> FindMessagesOfIdentity(IdentityAddress identityAddress, ListMessagesQuery request)
    {
        var addressOfActiveIdentity = _userContext.GetAddress();

        var query = _dbContext
            .Set<Message>()
            .AsQueryable()
            .IncludeAllReferences();

        if (request.Ids.Any())
            query = query.WithIdsIn(request.Ids);

        query = query.WithSenderOrRecipient(identityAddress);

        query = query.DoNotSendBeforePropertyIsNotInTheFuture();

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, request.PaginationFilter);

        return dbPaginationResult;
    }
}
