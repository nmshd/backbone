using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

public class ListMessagesResponse : PagedResponse<MessageDTO>
{
    public ListMessagesResponse(IEnumerable<Message> itemsOnPage, PaginationFilter previousPaginationFilter, int totalNumberOfItems, IdentityAddress activeIdentity, string didDomainName)
        : base(itemsOnPage.Select(i => new MessageDTO(i, activeIdentity, didDomainName)), previousPaginationFilter, totalNumberOfItems)
    {
    }
}
