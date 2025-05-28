using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplate;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.ConsumerApi.Controllers.Relationships;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class RelationshipTemplatesController : ApiControllerBase
{
    private readonly ApplicationConfiguration _configuration;

    public RelationshipTemplatesController(IMediator mediator, IOptions<ApplicationConfiguration> options) : base(mediator)
    {
        _configuration = options.Value;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipTemplateDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, [FromQuery] byte[]? password, CancellationToken cancellationToken)
    {
        var template = await _mediator.Send(new GetRelationshipTemplateQuery { Id = id, Password = password }, cancellationToken);
        return Ok(template);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListRelationshipTemplatesResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListAll([FromQuery] PaginationFilter paginationFilter, [FromQuery] ListRelationshipTemplatesQueryItem[]? templates,
        [FromQuery] IEnumerable<string> ids, CancellationToken cancellationToken)
    {
        // We keep this code for backwards compatibility reasons. In a few months the `templates`
        // parameter will become required, and the fallback to `ids` will be removed.
        templates = templates is { Length: > 0 } ? templates : ids.Select(id => new ListRelationshipTemplatesQueryItem { Id = id }).ToArray();

        var request = new ListRelationshipTemplatesQuery(paginationFilter, templates);

        request.PaginationFilter.PageSize ??= _configuration.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _configuration.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(_configuration.Pagination.MaxPageSize));

        var template = await _mediator.Send(request, cancellationToken);
        return Paged(template);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateRelationshipTemplateResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateRelationshipTemplateCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteRelationshipTemplateCommand { Id = id }, cancellationToken);
        return NoContent();
    }
}
