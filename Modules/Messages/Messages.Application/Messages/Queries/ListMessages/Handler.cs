using AutoMapper;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Messages.Application.Extensions;
using Messages.Application.Messages.DTOs;
using Messages.Domain.Entities;

namespace Messages.Application.Messages.Queries.ListMessages;

public class Handler : IRequestHandler<ListMessagesCommand, ListMessagesResponse>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly MessageService _messageService;
    private readonly IUserContext _userContext;

    public Handler(IDbContext dbContext, IUserContext userContext, IMapper mapper, MessageService messageService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _mapper = mapper;
        _messageService = messageService;
    }

    public async Task<ListMessagesResponse> Handle(ListMessagesCommand request, CancellationToken cancellationToken)
    {
        var dbPaginationResult = await FindMessagesOfIdentity(_userContext.GetAddress(), request);

        var response = new ListMessagesResponse(_mapper.Map<IEnumerable<MessageDTO>>(dbPaginationResult.ItemsOnPage), request.PaginationFilter, dbPaginationResult.TotalNumberOfItems);

        await _messageService.FillBodies(response);

        await _messageService.MarkMessagesAsReceived(dbPaginationResult.ItemsOnPage, cancellationToken);

        response.PrepareForActiveIdentity(_userContext.GetAddress());

        return response;
    }

    private async Task<DbPaginationResult<Message>> FindMessagesOfIdentity(IdentityAddress identityAddress, ListMessagesCommand request)
    {
        var addressOfActiveIdentity = _userContext.GetAddress();

        var query = _dbContext
            .Set<Message>()
            .AsQueryable()
            .IncludeAllReferences(addressOfActiveIdentity);

        if (request.Ids.Any())
            query = query.WithIdsIn(request.Ids);

        query = query.WithSenderOrRecipient(identityAddress);

        query = query.DoNotSendBeforePropertyIsNotInTheFuture();

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, request.PaginationFilter);

        return dbPaginationResult;
    }
}
