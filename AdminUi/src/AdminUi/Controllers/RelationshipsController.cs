using Backbone.AdminUi.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Application;
using Backbone.Modules.Relationships.Application.Relationships.DTOs;
using Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsByParticipantAddress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.AdminUi.Controllers;

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
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<RelationshipByParticipantAddressDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllRelationshipsByParticipantAddress([FromQuery] string participant, [FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;
        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var relationships = await _mediator.Send(new ListRelationshipsByParticipantAddressQuery(paginationFilter, participant), cancellationToken);
        return Paged(relationships);
    }
}
