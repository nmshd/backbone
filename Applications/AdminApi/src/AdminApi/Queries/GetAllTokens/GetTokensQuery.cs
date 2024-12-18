using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.AdminApi.Queries.GetAllTokens;

public record GetTokensQuery(string CreatedBy, PaginationFilter PaginationFilter) : IRequest<GetTokensResponse>;
