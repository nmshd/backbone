using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

public class ListMessagesQuery : IRequest<ListMessagesResponse>
{
    public ListMessagesQuery(PaginationFilter paginationFilter, IEnumerable<MessageId> ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public IEnumerable<MessageId> Ids { get; set; }
}
