using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
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
using Backbone.Modules.Relationships.Application.Relationships.Queries.GetRelationship;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationships;
using Backbone.Modules.Relationships.Application.RelationshipTemplates.Queries.GetRelationshipTemplate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.ConsumerApi.Controllers.Relationships;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class RelationshipsController : ApiControllerBase
{
    private readonly ApplicationConfiguration _configuration;

    public RelationshipsController(IMediator mediator, IOptions<ApplicationConfiguration> options) : base(mediator)
    {
        _configuration = options.Value;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRelationship(string id, CancellationToken cancellationToken)
    {
        var relationship = await _mediator.Send(new GetRelationshipQuery { Id = id }, cancellationToken);
        return Ok(relationship);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListRelationshipsResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListRelationships([FromQuery] PaginationFilter paginationFilter, [FromQuery] IEnumerable<string> ids, CancellationToken cancellationToken)
    {
        var request = new ListRelationshipsQuery { PaginationFilter = paginationFilter, Ids = ids.ToList() };

        request.PaginationFilter.PageSize ??= _configuration.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _configuration.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(_configuration.Pagination.MaxPageSize));

        var relationships = await _mediator.Send(request, cancellationToken);
        return Paged(relationships);
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateRelationshipResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateRelationship(CreateRelationshipCommand request, CancellationToken cancellationToken)
    {
        var relationshipTemplate = await _mediator.Send(new GetRelationshipTemplateQuery { Id = request.RelationshipTemplateId }, cancellationToken);
        await EnsurePeerIsActive(relationshipTemplate.CreatedBy, cancellationToken);

        var relationship = await _mediator.Send(request, cancellationToken);
        return Created(relationship);
    }

    [HttpPut("{id}/Accept")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<AcceptRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptRelationship([FromRoute] string id, [FromBody] AcceptRelationshipRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new AcceptRelationshipCommand { RelationshipId = id, CreationResponseContent = request.CreationResponseContent }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Reject")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RejectRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectRelationship([FromRoute] string id, [FromBody] RejectRelationshipRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RejectRelationshipCommand { RelationshipId = id, CreationResponseContent = request.CreationResponseContent }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Revoke")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RevokeRelationshipResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeRelationship([FromRoute] string id, [FromBody] RevokeRelationshipRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RevokeRelationshipCommand { RelationshipId = id, CreationResponseContent = request.CreationResponseContent }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Terminate")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RelationshipMetadataDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TerminateRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
        var relationship = await _mediator.Send(new TerminateRelationshipCommand { RelationshipId = id }, cancellationToken);
        return Ok(relationship);
    }

    [HttpPut("{id}/Reactivate")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RequestRelationshipReactivationResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RelationshipReactivationRequest([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RequestRelationshipReactivationCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Reactivate/Revoke")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RevokeRelationshipReactivationResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeRelationshipReactivation([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RevokeRelationshipReactivationCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Reactivate/Accept")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<AcceptRelationshipReactivationResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptReactivationOfRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new AcceptRelationshipReactivationCommand { RelationshipId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/Reactivate/Reject")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RejectRelationshipReactivationResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectReactivationOfRelationship([FromRoute] string id, CancellationToken cancellationToken)
    {
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
    public async Task<IActionResult> CanEstablishRelationship([FromQuery(Name = "peer")] string peerAddress, CancellationToken cancellationToken)
    {
        var peerIdentity = await _mediator.Send(new GetIdentityQuery { Address = peerAddress }, cancellationToken);

        var response = peerIdentity.Status is not IdentityStatus.Active
            ? new CanEstablishRelationshipResponse { CanCreate = false, Code = ApplicationErrors.Relationship.PeerIsToBeDeleted().Code }
            : await _mediator.Send(new CanEstablishRelationshipQuery { PeerAddress = peerAddress }, cancellationToken);

        return Ok(response);
    }

    private async Task EnsurePeerIsActive(string peerIdentityAddress, CancellationToken cancellationToken)
    {
        var peerIdentity = await _mediator.Send(new GetIdentityQuery { Address = peerIdentityAddress }, cancellationToken);
        if (peerIdentity.Status is not IdentityStatus.Active)
            throw new ApplicationException(ApplicationErrors.Relationship.PeerIsToBeDeleted());
    }
}

public class AcceptRelationshipRequest
{
    public byte[]? CreationResponseContent { get; set; } = [];
}

public class RejectRelationshipRequest
{
    public byte[]? CreationResponseContent { get; set; } = [];
}

public class RevokeRelationshipRequest
{
    public byte[]? CreationResponseContent { get; set; } = [];
}
