using Backbone.API.Mvc;
using Backbone.API.Mvc.ControllerAttributes;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationshipTemplate;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.DeleteRelationshipTemplate;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Validation.AspNetCore;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class RelationshipTemplatesController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public RelationshipTemplatesController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipTemplateDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(RelationshipTemplateId id)
    {
        var template = await _mediator.Send(new GetRelationshipTemplateQuery { Id = id });
        return Ok(template);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListRelationshipTemplatesResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<RelationshipTemplateId> ids)
    {
        var request = new ListRelationshipTemplatesQuery(paginationFilter, ids);

        request.PaginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var template = await _mediator.Send(request);
        return Paged(template);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateRelationshipTemplateResponse>),
        StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateRelationshipTemplateCommand request)
    {
        var response = await _mediator.Send(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpDelete("{id}")]
    [ProducesError(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(RelationshipTemplateId id)
    {
        await _mediator.Send(new DeleteRelationshipTemplateCommand { Id = id });
        return NoContent();
    }
}