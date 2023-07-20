using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Relationships.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
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
    public async Task<IActionResult> GetById(RelationshipTemplateId id, CancellationToken cancellationToken)
    {
        var template = await _mediator.Send(new GetRelationshipTemplateQuery { Id = id }, cancellationToken);
        return Ok(template);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListRelationshipTemplatesResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<RelationshipTemplateId> ids, CancellationToken cancellationToken)
    {
        var request = new ListRelationshipTemplatesQuery(paginationFilter, ids);

        request.PaginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var template = await _mediator.Send(request, cancellationToken);
        return Paged(template);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateRelationshipTemplateResponse>),
        StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateRelationshipTemplateCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}
