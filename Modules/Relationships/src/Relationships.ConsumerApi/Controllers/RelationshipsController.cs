using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.AcceptRelationshipReactivation;
using Backbone.Modules.Relationships.Application.Relationships.Commands.CreateRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.DecomposeRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RejectRelationshipReactivation;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RequestRelationshipReactivation;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Commands.RevokeRelationshipReactivation;
using Backbone.Modules.Relationships.Application.Relationships.Commands.TerminateRelationship;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.Relationships.Queries.CanEstablishRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Queries.GetPeerOfActiveIdentityInRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;
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
        var relationshipTemplate = await _mediator.Send(new GetRelationshipTemplateQuery { Id = request.RelationshipTemplateId }, cancellationToken);
        await EnsurePeerIsNotToBeDeleted(relationshipTemplate.CreatedBy, cancellationToken);

        var relationship = await _mediator.Send(request, cancellationToken);
        return Created(relationship);
    }

    [HttpPut("{id}/Accept")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<AcceptRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptRelationship([FromRoute] string id, [FromBody] AcceptRelationshipRequest request, CancellationToken cancellationToken)
    {
        var peerOfActiveIdentityInRelationshipResponse = await _mediator.Send(new GetPeerOfActiveIdentityInRelationshipQuery { Id = id }, cancellationToken);
        await EnsurePeerIsNotToBeDeleted(peerOfActiveIdentityInRelationshipResponse.IdentityAddress, cancellationToken);

        var response = await _mediator.Send(new AcceptRelationshipCommand { RelationshipId = id, CreationResponseContent = request.CreationResponseContent }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Reject")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RejectRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectRelationship([FromRoute] string id, [FromBody] RejectRelationshipRequest request, CancellationToken cancellationToken)
    {
        var peerOfActiveIdentityInRelationshipResponse = await _mediator.Send(new GetPeerOfActiveIdentityInRelationshipQuery { Id = id }, cancellationToken);
        await EnsurePeerIsNotToBeDeleted(peerOfActiveIdentityInRelationshipResponse.IdentityAddress, cancellationToken);

        var response = await _mediator.Send(new RejectRelationshipCommand { RelationshipId = id, CreationResponseContent = request.CreationResponseContent }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Revoke")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RevokeRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeRelationship([FromRoute] string id, [FromBody] RevokeRelationshipRequest request, CancellationToken cancellationToken)
    {
        var peerOfActiveIdentityInRelationshipResponse = await _mediator.Send(new GetPeerOfActiveIdentityInRelationshipQuery { Id = id }, cancellationToken);
        await EnsurePeerIsNotToBeDeleted(peerOfActiveIdentityInRelationshipResponse.IdentityAddress, cancellationToken);

        var response = await _mediator.Send(new RevokeRelationshipCommand { RelationshipId = id, CreationResponseContent = request.CreationResponseContent }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Terminate")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TerminateRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
        var peerOfActiveIdentityInRelationshipResponse = await _mediator.Send(new GetPeerOfActiveIdentityInRelationshipQuery { Id = id }, cancellationToken);
        await EnsurePeerIsNotToBeDeleted(peerOfActiveIdentityInRelationshipResponse.IdentityAddress, cancellationToken);

        var relationship = await _mediator.Send(new TerminateRelationshipCommand { RelationshipId = id }, cancellationToken);
        return Ok(relationship);
    }

    [HttpPut("{id}/Reactivate")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RequestRelationshipReactivationResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RelationshipReactivationRequest([FromRoute] string id, CancellationToken cancellationToken)
    {
        var peerOfActiveIdentityInRelationshipResponse = await _mediator.Send(new GetPeerOfActiveIdentityInRelationshipQuery { Id = id }, cancellationToken);
        await EnsurePeerIsNotToBeDeleted(peerOfActiveIdentityInRelationshipResponse.IdentityAddress, cancellationToken);

        var response = await _mediator.Send(new RequestRelationshipReactivationCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Reactivate/Revoke")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RevokeRelationshipReactivationResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeRelationshipReactivation([FromRoute] string id, CancellationToken cancellationToken)
    {
        var peerOfActiveIdentityInRelationshipResponse = await _mediator.Send(new GetPeerOfActiveIdentityInRelationshipQuery { Id = id }, cancellationToken);
        await EnsurePeerIsNotToBeDeleted(peerOfActiveIdentityInRelationshipResponse.IdentityAddress, cancellationToken);

        var response = await _mediator.Send(new RevokeRelationshipReactivationCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Reactivate/Accept")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<AcceptRelationshipReactivationResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptReactivationOfRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
        var peerOfActiveIdentityInRelationshipResponse = await _mediator.Send(new GetPeerOfActiveIdentityInRelationshipQuery { Id = id }, cancellationToken);
        await EnsurePeerIsNotToBeDeleted(peerOfActiveIdentityInRelationshipResponse.IdentityAddress, cancellationToken);

        var response = await _mediator.Send(new AcceptRelationshipReactivationCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Reactivate/Reject")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RejectRelationshipReactivationResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectReactivationOfRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
        var peerOfActiveIdentityInRelationshipResponse = await _mediator.Send(new GetPeerOfActiveIdentityInRelationshipQuery { Id = id }, cancellationToken);
        await EnsurePeerIsNotToBeDeleted(peerOfActiveIdentityInRelationshipResponse.IdentityAddress, cancellationToken);

        var response = await _mediator.Send(new RejectRelationshipReactivationCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Decompose")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<DecomposeRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DecomposeRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new DecomposeRelationshipCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpGet("CanCreate")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CanEstablishRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CanEstablishRelationship([FromQuery] string peer, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CanEstablishRelationshipQuery { PeerAddress = peer }, cancellationToken);
        return Ok(response);
    }

    private async Task EnsurePeerIsNotToBeDeleted(string peerIdentityAddress, CancellationToken cancellationToken)
    {
        var peerIdentity = await _mediator.Send(new GetIdentityQuery(peerIdentityAddress), cancellationToken);
        if (peerIdentity.Status is IdentityStatus.ToBeDeleted)
            throw new ApplicationException(ApplicationErrors.Relationship.PeerIsToBeDeleted(peerIdentity.Address));
    }
}

public class AcceptRelationshipRequest
{
    public byte[]? CreationResponseContent { get; set; } = Array.Empty<byte>();
}

public class RejectRelationshipRequest
{
    public byte[]? CreationResponseContent { get; set; } = Array.Empty<byte>();
}

public class RevokeRelationshipRequest
{
    public byte[]? CreationResponseContent { get; set; } = Array.Empty<byte>();
}
