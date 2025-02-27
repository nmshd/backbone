using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.ConsumerApi.Controllers;

[Route("api/poc/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
[ApiExplorerSettings(IgnoreApi = true)] // don't show this endpoints of this controller in the API docs as it's just a PoC
public class PublicRelationshipTemplateReferencesController : ApiControllerBase
{
    private readonly Configuration _options;

    public PublicRelationshipTemplateReferencesController(IMediator mediator, IOptions<Configuration> options) : base(mediator)
    {
        _options = options.Value;
    }

    [HttpGet]
    public IActionResult ListPublicRelationshipTemplateReferences(IUserContext userContext)
    {
        var clientId = userContext.GetClientId();
        var response = _options.PublicRelationshipTemplateReferences.GetValueOrDefault(clientId) ?? [];

        return Ok(response);
    }
}
