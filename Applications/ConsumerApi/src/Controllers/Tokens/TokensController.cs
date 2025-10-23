using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.ConsumerApi.Versions;
using Backbone.Modules.Tokens.Application;
using Backbone.Modules.Tokens.Application.Tokens.Commands.CreateToken;
using Backbone.Modules.Tokens.Application.Tokens.Commands.DeleteToken;
using Backbone.Modules.Tokens.Application.Tokens.Commands.UpdateTokenContent;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Application.Tokens.Queries.GetToken;
using Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.ConsumerApi.Controllers.Tokens;

[V2]
[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class TokensController : ApiControllerBase
{
    private readonly ApplicationConfiguration _configuration;

    public TokensController(IMediator mediator, IOptions<ApplicationConfiguration> options) : base(mediator)
    {
        _configuration = options.Value;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateTokenResponse>), StatusCodes.Status201Created)]
    [AllowAnonymous]
    public async Task<IActionResult> CreateToken(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetToken), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<TokenDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<IActionResult> GetToken([FromRoute] string id, [FromQuery] byte[]? password, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetTokenQuery { Id = id, Password = password }, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<TokenDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListTokens([FromQuery] PaginationFilter paginationFilter, [FromQuery] ListTokensQueryItem[]? tokens, [FromQuery] List<string> ids,
        CancellationToken cancellationToken)
    {
        // We keep this code for backwards compatibility reasons. In a few months the `ids`
        // parameter will become required again, and the fallback to `tokens` will be removed.
        ids = ids.Count != 0 ? ids : tokens?.Select(t => t.Id).ToList() ?? [];

        var request = new ListTokensQuery { PaginationFilter = paginationFilter, Ids = ids };

        paginationFilter.PageSize ??= _configuration.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _configuration.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(_configuration.Pagination.MaxPageSize));

        var response = await _mediator.Send(request, cancellationToken);

        return Paged(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteToken([FromRoute] string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteTokenCommand { Id = id }, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/UpdateContent")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<UpdateTokenContentResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTokenContent([FromRoute] string id, [FromBody] UpdateTokenContentRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTokenContentCommand
        {
            TokenId = id,
            NewContent = request.NewContent,
            Password = request.Password
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }
}

public class UpdateTokenContentRequest
{
    public required byte[] NewContent { get; init; }
    public byte[]? Password { get; init; }
}

public class ListTokensQueryItem
{
    public required string Id { get; set; }
}
