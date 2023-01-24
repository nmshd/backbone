using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Messages.Application.Messages.DTOs;

namespace Messages.Application.Messages.Queries.ListMessages;

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
