using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Module.Features.Messages.Shared;

namespace Backbone.Modules.Messages.Module.Features.Messages.ListMessages;

public class ListMessagesResponse : PagedResponse<MessageDTO>
{
    public ListMessagesResponse(DbPaginationResult<Message> result, PaginationFilter previousFilter, IdentityAddress activeIdentity, string didDomainName)
        : base(result.ItemsOnPage.Select(message => new MessageDTO(message, activeIdentity, didDomainName)), previousFilter, result.TotalNumberOfItems)
    {
    }
}
