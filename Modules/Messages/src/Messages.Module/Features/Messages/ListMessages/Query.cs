using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Messages.Module.Features.Messages.ListMessages;

public class ListMessagesQuery : IRequest<ListMessagesResponse>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required IEnumerable<string> Ids { get; init; }
}
