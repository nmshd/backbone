using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
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

public class PublicRelationshipTemplateReferenceDefinition
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string TruncatedReference { get; set; } = string.Empty;
}
