using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Messages.Domain.Ids;
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
