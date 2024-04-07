using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.TerminateRelationship;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Relationships.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class RelationshipsController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public RelationshipsController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRelationship(RelationshipId id, CancellationToken cancellationToken)
    {
        var relationship = await _mediator.Send(new GetRelationshipQuery { Id = id }, cancellationToken);
        return Ok(relationship);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListRelationshipsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListRelationships([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<RelationshipId> ids, CancellationToken cancellationToken)
    {
        var request = new ListRelationshipsQuery(paginationFilter, ids);

        request.PaginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var relationships = await _mediator.Send(request, cancellationToken);
        return Paged(relationships);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateRelationship(CreateRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _mediator.Send(request, cancellationToken);
        return Created(relationship);
    }

    [HttpPut("{id}/Accept")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<AcceptRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new AcceptRelationshipCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Reject")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RejectRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RejectRelationshipCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Revoke")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RevokeRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RevokeRelationshipCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Terminate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<TerminateRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TerminateRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new TerminateRelationshipCommand() { RelationshipId = id }, cancellationToken);
        return NoContent();
    }
}
