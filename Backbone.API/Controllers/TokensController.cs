using Backbone.API;
using Backbone.API.Mvc;
using Backbone.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tokens.Application;
using Tokens.Application.Tokens.Commands.CreateToken;
using Tokens.Application.Tokens.Commands.DeleteToken;
using Tokens.Application.Tokens.DTOs;
using Tokens.Application.Tokens.Queries.GetToken;
using Tokens.Application.Tokens.Queries.ListTokens;
using Tokens.Domain.Entities;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Tokens.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
public class TokensController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public TokensController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateTokenResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateToken(CreateTokenCommand request)
    {
        var response = await _mediator.Send(request);
        return CreatedAtAction(nameof(GetToken), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<TokenDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<IActionResult> GetToken([FromRoute] TokenId id)
    {
        var response = await _mediator.Send(new GetTokenCommand { Id = id });
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<TokenDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListTokens([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<TokenId> ids)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var response = await _mediator.Send(new ListTokensQuery(paginationFilter, ids));

        return Paged(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteToken(TokenId id)
    {
        await _mediator.Send(new DeleteTokenCommand { Id = id });

        return NoContent();
    }
}