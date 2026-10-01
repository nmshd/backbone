using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Module.Features.Tokens.Shared;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ListTokensByIdentity;

public record ListTokensByIdentityQuery(string CreatedBy, PaginationFilter PaginationFilter) : IRequest<ListTokensResponse>;
