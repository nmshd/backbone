using Backbone.Modules.Devices.Application.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Admin.API.Mvc;
using Microsoft.Extensions.Options;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using OpenIddict.Validation.AspNetCore;

namespace Admin.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class IdentitiesController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public IdentitiesController(
        IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<IdentityDTO>), StatusCodes.Status200OK)]
    public async Task<List<IdentityDTO>> GetIdentitiesAsync([FromQuery] PaginationFilter paginationFilter)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;
        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var query = await _mediator.Send(new ListIdentitiesQuery(paginationFilter));
        return query.Identities;
    }
}
