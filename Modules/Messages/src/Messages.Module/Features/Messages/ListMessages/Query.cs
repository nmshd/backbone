using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Messages.Module.Features.Messages.ListMessages;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListMessagesQuery")]
public class Query : IRequest<Response>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public required IEnumerable<string> Ids { get; init; }
}
