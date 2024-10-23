using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ListMessages;

public class ListMessagesResponse : PagedResponse<MessageDTO>
{
    public ListMessagesResponse(DbPaginationResult<Message> dbPaginationResult, PaginationFilter previousPaginationFilter, IdentityAddress activeIdentity, string didDomainName)
        : base(dbPaginationResult.ItemsOnPage.Select(i => new MessageDTO(i, activeIdentity, didDomainName)), previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
