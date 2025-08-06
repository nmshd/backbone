using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Application.Tokens.Queries.Shared;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensQuery : IRequest<ListTokensResponse>
{
    public required PaginationFilter PaginationFilter { get; init; }
    public List<string> Ids { get; set; } = [];
}
