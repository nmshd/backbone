using System.Text.Json;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Commands.CreateRelationshipTemplate;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.ListRelationshipTemplates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Relationships.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class RelationshipTemplatesController : ApiControllerBase
{
    private readonly ApplicationOptions _options;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public RelationshipTemplatesController(IMediator mediator, IOptions<ApplicationOptions> options, IOptions<JsonOptions> jsonOptions) : base(mediator)
    {
        _options = options.Value;
        _jsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipTemplateDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, [FromQuery] byte[]? password, CancellationToken cancellationToken)
    {
        var template = await _mediator.Send(new GetRelationshipTemplateQuery { Id = id, Password = password }, cancellationToken);
        return Ok(template);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListRelationshipTemplatesResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PaginationFilter paginationFilter, [FromQuery] string? templates, [FromQuery] IEnumerable<string> ids, CancellationToken cancellationToken)
    {
        var relationshipTemplateQueryItems =
            templates != null ? JsonSerializer.Deserialize<List<RelationshipTemplateQueryItem>>(templates) : ids!.Select(id => new RelationshipTemplateQueryItem { Id = id }).ToList();

        List<RelationshipTemplateQueryItem>? relationshipTemplateQueryItems;

        if (templates != null)
        {
            try
            {
                relationshipTemplateQueryItems = JsonSerializer.Deserialize<List<RelationshipTemplateQueryItem>>(templates, _jsonSerializerOptions)
            }
            catch (JsonException)
            {
                throw new ApplicationException(ApplicationErrors.RelationshipTemplate.InvalidRelationshipTemplateQueryItem());
            }
        }
        else
        {
            relationshipTemplateQueryItems = ids!.Select(id => new RelationshipTemplateQueryItem { Id = id }).ToList();
        }

        var request = new ListRelationshipTemplatesQuery(paginationFilter, relationshipTemplateQueryItems);

        request.PaginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

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
