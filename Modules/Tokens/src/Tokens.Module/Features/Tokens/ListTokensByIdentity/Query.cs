using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Module.Features.Tokens.Shared;
using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.ListTokensByIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListTokensByIdentityQuery")]
public record Query(string CreatedBy, PaginationFilter PaginationFilter) : IRequest<Response>;
