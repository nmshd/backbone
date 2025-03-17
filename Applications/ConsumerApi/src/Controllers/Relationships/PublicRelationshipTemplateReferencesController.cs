using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Relationships.Module;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.ConsumerApi.Controllers.Relationships;

[Route("api/poc/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
[ApiExplorerSettings(IgnoreApi = true)] // don't show the endpoints of this controller in the API docs as it's just a PoC
public class PublicRelationshipTemplateReferencesController : ApiControllerBase
{
    private readonly Dictionary<string, IEnumerable<PublicRelationshipTemplateReferenceDefinition>> _publicRelationshipTemplateReferenceDefinitions = [];

    public PublicRelationshipTemplateReferencesController(IMediator mediator, IConfiguration configuration) : base(mediator)
    {
        configuration.GetSection("Modules:Relationships:PublicRelationshipTemplateReferences").Bind(_publicRelationshipTemplateReferenceDefinitions);
    }

    [HttpGet]
    public IActionResult ListPublicRelationshipTemplateReferences(IUserContext userContext)
    {
        var clientId = userContext.GetClientId();
        var response = _publicRelationshipTemplateReferenceDefinitions.GetValueOrDefault(clientId) ?? [];

        return Ok(response);
    }
}
