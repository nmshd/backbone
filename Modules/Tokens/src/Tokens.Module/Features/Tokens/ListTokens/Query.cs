using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Module.Features.Tokens.Shared;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ListTokens;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListTokensQuery")]
public class Query : IRequest<Response>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public List<string> Ids { get; set; } = [];
}
