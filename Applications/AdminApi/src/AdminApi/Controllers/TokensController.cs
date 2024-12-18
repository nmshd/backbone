using Backbone.AdminApi.Configuration;
using Backbone.AdminApi.Queries.GetAllTokens;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace Backbone.AdminApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class TokensController(IMediator mediator, IOptions<AdminConfiguration> options) : ApiControllerBase(mediator)
{
    // The Admin API has a new endpoint GET /Tokens?createdBy={identityAddress}, which returns all Tokens of created by the identity with the given address.
    // All properties of the Token are returned.
    [HttpGet]
    [ProducesResponseType(typeof(List<Token>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTokens([FromQuery] PaginationFilter paginationFilter, [FromQuery] string createdBy, CancellationToken cancellationToken)
    {
        if (paginationFilter.PageSize != null)
        {
            var maxPageSize = options.Value.Modules.Devices.Application.Pagination.MaxPageSize;

            if (paginationFilter.PageSize > maxPageSize)
            {
                throw new BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(maxPageSize));
            }
        }

        var request = new GetTokensQuery(createdBy, paginationFilter);
        var pagedResult = await _mediator.Send(request, cancellationToken);

        return Paged(pagedResult);
    }
}
