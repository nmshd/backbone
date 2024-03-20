using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationshipChangeRequest;
using Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationshipChangeRequest;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationshipChangeRequest;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.Relationships.Queries.GetChange;
using Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListChanges;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;
using Backbone.Modules.Relationships.Common;
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

    [HttpGet("Changes")]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListChangesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListChanges(
        [FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<RelationshipChangeId> ids,
        [FromQuery] OptionalDateRange? createdAt,
        [FromQuery] OptionalDateRange? completedAt,
        [FromQuery] OptionalDateRange? modifiedAt,
        [FromQuery] bool? onlyPeerChanges,
        [FromQuery] IdentityAddress? createdBy,
        [FromQuery] IdentityAddress? completedBy,
        [FromQuery] string? status,
        [FromQuery] string? type, CancellationToken cancellationToken)
    {
        var query = new ListChangesQuery(
            paginationFilter,
            ids,
            createdAt,
            completedAt,
            modifiedAt,
            status == null ? null : Enum.Parse<RelationshipChangeStatus>(status),
            type == null ? null : Enum.Parse<RelationshipChangeType>(type),
            createdBy,
            completedBy,
            onlyPeerChanges ?? false);

        query.PaginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var changes = await _mediator.Send(query, cancellationToken);
        return Paged(changes);
    }

    [HttpGet("Changes/{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipChangeDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChangeById(RelationshipChangeId id, CancellationToken cancellationToken)
    {
        var relationship = await _mediator.Send(new GetChangeQuery { Id = id }, cancellationToken);
        return Ok(relationship);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateRelationshipResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateRelationship(CreateRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationship = await _mediator.Send(request, cancellationToken);
        return Created(relationship);
    }

    [HttpPut("{relationshipId}/Changes/{changeId}/Accept")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipChangeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptRelationshipChange([FromRoute] RelationshipId relationshipId,
        [FromRoute] RelationshipChangeId changeId, CompleteRelationshipChangeRequest request, CancellationToken cancellationToken)
    {
        var change = await _mediator.Send(new AcceptRelationshipChangeRequestCommand
        {
            Id = relationshipId,
            ChangeId = changeId,
            ResponseContent = request.Content
        }, cancellationToken);

        return Ok(change);
    }

    [HttpPut("{relationshipId}/Changes/{changeId}/Reject")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipChangeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectRelationshipChange([FromRoute] RelationshipId relationshipId,
        [FromRoute(Name = "changeId")] RelationshipChangeId changeId, CompleteRelationshipChangeRequest request, CancellationToken cancellationToken)
    {
        var change = await _mediator.Send(new RejectRelationshipChangeRequestCommand
        {
            Id = relationshipId,
            ChangeId = changeId,
            ResponseContent = request.Content
        }, cancellationToken);

        return Ok(change);
    }

    [HttpPut("{relationshipId}/Changes/{changeId}/Revoke")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipChangeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeRelationshipChange([FromRoute] RelationshipId relationshipId,
        [FromRoute(Name = "changeId")] RelationshipChangeId changeId, CompleteRelationshipChangeRequest request, CancellationToken cancellationToken)
    {
        var change = await _mediator.Send(new RevokeRelationshipChangeRequestCommand
        {
            Id = relationshipId,
            ChangeId = changeId,
            ResponseContent = request.Content
        }, cancellationToken);

        return Ok(change);
    }
}

public class CreateRelationshipChangeRequest
{
    public RelationshipChangeType Type { get; set; }
}

public class CompleteRelationshipChangeRequest
{
    public byte[] Content { get; set; } = Array.Empty<byte>();
}
