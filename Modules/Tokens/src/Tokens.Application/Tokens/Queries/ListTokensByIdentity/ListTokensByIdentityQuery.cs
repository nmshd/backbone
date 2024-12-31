using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Application.Tokens.Queries.Shared;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokensByIdentity;

public record ListTokensByIdentityQuery(string CreatedBy, PaginationFilter PaginationFilter) : IRequest<ListTokensResponse>;
