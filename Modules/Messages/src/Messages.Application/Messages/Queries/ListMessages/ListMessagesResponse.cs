using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Messages.DTOs;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

public class ListMessagesResponse : PagedResponse<MessageDTO>
{
    public ListMessagesResponse(IEnumerable<MessageDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }

    public void PrepareForActiveIdentity(IdentityAddress activeIdentity)
    {
        foreach (var dto in this)
        {
            dto.PrepareForActiveIdentity(activeIdentity);
        }
    }
}
