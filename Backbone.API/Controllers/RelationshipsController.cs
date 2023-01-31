using Backbone.API.Mvc;
using Backbone.API.Mvc.ControllerAttributes;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationshipChangeRequest;
using Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationshipTerminationRequest;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationshipChangeRequest;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationshipChangeRequest;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.Relationships.Queries.GetChange;
using Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListChanges;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;
using Backbone.Modules.Relationships.Common;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
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
    public async Task<IActionResult> GetRelationship(RelationshipId id)
    {
        var relationship = await _mediator.Send(new GetRelationshipQuery { Id = id });
        return Ok(relationship);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListRelationshipsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListRelationships([FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<RelationshipId> ids)
    {
        var request = new ListRelationshipsQuery(paginationFilter, ids);

        request.PaginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var relationships = await _mediator.Send(request);
        return Paged(relationships);
    }

    [HttpGet("Changes")]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListChangesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListChanges(
        [FromQuery] PaginationFilter paginationFilter,
        [FromQuery] IEnumerable<RelationshipChangeId> ids,
        [FromQuery] OptionalDateRange createdAt,
        [FromQuery] OptionalDateRange completedAt,
        [FromQuery] OptionalDateRange modifiedAt,
        [FromQuery] bool? onlyPeerChanges,
        [FromQuery] IdentityAddress createdBy,
        [FromQuery] IdentityAddress completedBy,
        [FromQuery] string? status,
        [FromQuery] string? type)
    {
        var request = new ListChangesQuery(
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

        request.PaginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var changes = await _mediator.Send(request);
        return Paged(changes);
    }

    [HttpGet("Changes/{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipChangeDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChangeById(RelationshipChangeId id)
    {
        var relationship = await _mediator.Send(new GetChangeQuery { Id = id });
        return Ok(relationship);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateRelationshipResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateRelationship(CreateRelationshipCommand request)
    {
        var relationship = await _mediator.Send(request);
        return Created(relationship);
    }

    [HttpPost("{relationshipId}/Changes")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipChangeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateRelationshipChange([FromRoute] RelationshipId relationshipId,
        CreateRelationshipChangeRequest request)
    {
        var change = request.Type switch
        {
            RelationshipChangeType.Termination => await _mediator.Send(
                new CreateRelationshipTerminationRequestCommand { Id = relationshipId }
            ),
            RelationshipChangeType.TerminationCancellation => throw new NotImplementedException(),
            RelationshipChangeType.Creation => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };

        return Ok(change);
    }

    [HttpPut("{relationshipId}/Changes/{changeId}/Accept")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipChangeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptRelationshipChange([FromRoute] RelationshipId relationshipId,
        [FromRoute] RelationshipChangeId changeId, CompleteRelationshipChangeRequest request)
    {
        var change = await _mediator.Send(new AcceptRelationshipChangeRequestCommand
        {
            Id = relationshipId,
            ChangeId = changeId,
            ResponseContent = request.Content
        });

        return Ok(change);
    }

    [HttpPut("{relationshipId}/Changes/{changeId}/Reject")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipChangeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectRelationshipChange([FromRoute] RelationshipId relationshipId,
        [FromRoute(Name = "changeId")] RelationshipChangeId changeId, CompleteRelationshipChangeRequest request)
    {
        var change = await _mediator.Send(new RejectRelationshipChangeRequestCommand
        {
            Id = relationshipId,
            ChangeId = changeId,
            ResponseContent = request.Content
        });

        return Ok(change);
    }

    [HttpPut("{relationshipId}/Changes/{changeId}/Revoke")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipChangeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeRelationshipChange([FromRoute] RelationshipId relationshipId,
        [FromRoute(Name = "changeId")] RelationshipChangeId changeId, CompleteRelationshipChangeRequest request)
    {
        var change = await _mediator.Send(new RevokeRelationshipChangeRequestCommand
        {
            Id = relationshipId,
            ChangeId = changeId,
            ResponseContent = request.Content
        });

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