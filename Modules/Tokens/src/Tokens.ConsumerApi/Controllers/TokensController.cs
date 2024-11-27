using System.Text.Json;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;
using Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteToken;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;
using Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;
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
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public TokensController(IMediator mediator, IOptions<ApplicationOptions> options, IOptions<JsonOptions> jsonOptions) : base(mediator)
    {
        _options = options.Value;
        _jsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;
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
    public async Task<IActionResult> GetToken([FromRoute] string id, [FromQuery] byte[]? password, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetTokenQuery { Id = id, Password = password }, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<TokenDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListTokens([FromQuery] PaginationFilter paginationFilter, [FromQuery] string? tokens,
        [FromQuery] IEnumerable<string> ids, CancellationToken cancellationToken)
    {
        List<ListTokensQueryItem>? tokenQueryItems;

        if (tokens != null)
        {
            try
            {
                tokenQueryItems = JsonSerializer.Deserialize<List<ListTokensQueryItem>>(tokens, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                throw new ApplicationException(GenericApplicationErrors.Validation.InputCannotBeParsed(ex.Message));
            }
        }
        else
        {
            tokenQueryItems = ids.Select(id => new ListTokensQueryItem { Id = id }).ToList();
        }

        var request = new ListTokensQuery(paginationFilter, tokenQueryItems);

        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var response = await _mediator.Send(request, cancellationToken);

        return Paged(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteToken([FromRoute] string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteTokenCommand { Id = id }, cancellationToken);
        return NoContent();
    }
}
