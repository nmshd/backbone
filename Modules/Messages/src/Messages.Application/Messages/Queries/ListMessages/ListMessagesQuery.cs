using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

public class ListMessagesQuery : IRequest<ListMessagesResponse>
{
    public ListMessagesQuery(PaginationFilter paginationFilter, IEnumerable<string> ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public IEnumerable<string> Ids { get; set; }
}
