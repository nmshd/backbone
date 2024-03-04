using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;
using Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Tokens.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class TokensController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public TokensController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateTokenResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateToken(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetToken), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<TokenDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<IActionResult> GetToken([FromRoute] TokenId id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetTokenQuery { Id = id }, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<TokenDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListTokens([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<TokenId> ids, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var response = await _mediator.Send(new ListTokensQuery(paginationFilter, ids), cancellationToken);

        return Paged(response);
    }
}
