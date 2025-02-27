﻿using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokensByIdentity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.AdminApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class TokensController(IMediator mediator, IOptions<ApplicationOptions> options) : ApiControllerBase(mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<List<TokenDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListTokensByIdentity([FromQuery] PaginationFilter paginationFilter, [FromQuery] string createdBy, CancellationToken cancellationToken)
    {
        if (paginationFilter.PageSize != null)
        {
            var maxPageSize = options.Value.Pagination.MaxPageSize;

            if (paginationFilter.PageSize > maxPageSize)
            {
                throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(maxPageSize));
            }
        }

        var request = new ListTokensByIdentityQuery(createdBy, paginationFilter);
        var pagedResult = await _mediator.Send(request, cancellationToken);

        return Paged(pagedResult);
    }
}
