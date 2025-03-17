using Backbone.AdminApi.Infrastructure.DTOs;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.AdminApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class RelationshipsController : ApiControllerBase
{
    private readonly AdminApiDbContext _adminApiDbContext;
    private readonly ApplicationConfiguration _configuration;


    public RelationshipsController(IMediator mediator, IOptions<ApplicationConfiguration> options, AdminApiDbContext adminApiDbContext) : base(mediator)
    {
        _adminApiDbContext = adminApiDbContext;
        _configuration = options.Value;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<RelationshipDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRelationships([FromQuery] string participant, [FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _configuration.Pagination.DefaultPageSize;
        if (paginationFilter.PageSize > _configuration.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_configuration.Pagination.MaxPageSize));

        var relationshipOverviews = await _adminApiDbContext.RelationshipOverviews
            .Where(r => r.To == participant || r.From == participant)
            .OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        var relationshipItems = relationshipOverviews.ItemsOnPage.Select(i => new RelationshipDTO(participant, i));

        return Paged(new PagedResponse<RelationshipDTO>(relationshipItems, paginationFilter, relationshipOverviews.TotalNumberOfItems));
    }
}
