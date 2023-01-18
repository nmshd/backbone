using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Messages.Common;
using Messages.Domain.Ids;

namespace Messages.Application.Messages.Queries.ListMessages;

public class ListMessagesCommand : IRequest<ListMessagesResponse>
{
    public ListMessagesCommand(PaginationFilter paginationFilter, IEnumerable<MessageId> ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public IEnumerable<MessageId> Ids { get; set; }
}
