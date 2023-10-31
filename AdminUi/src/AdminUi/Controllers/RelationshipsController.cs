using AdminUi.Infrastructure.Persistence.Database;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsByParticipantAddress;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AdminUi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class RelationshipsController : ApiControllerBase
{
    private readonly AdminUiDbContext _adminUiDbContext;
    private readonly ApplicationOptions _options;


    public RelationshipsController(IMediator mediator, IOptions<ApplicationOptions> options, AdminUiDbContext adminUiDbContext) : base(mediator)
    {
        _adminUiDbContext = adminUiDbContext;
        _options = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<RelationshipDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllRelationshipsByParticipantAddress([FromQuery] string participant, [FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;
        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var relationships = await _mediator.Send(new ListRelationshipsByParticipantAddressQuery(paginationFilter, participant), cancellationToken);
        return Paged(relationships);
    }
}
