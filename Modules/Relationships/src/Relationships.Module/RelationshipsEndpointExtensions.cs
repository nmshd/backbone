using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Relationships.Module.Features.PublicRelationshipTemplateReferences.ListPublicRelationshipTemplateReferences;
using Backbone.Modules.Relationships.Module.Features.Relationships.AcceptRelationship;
using Backbone.Modules.Relationships.Module.Features.Relationships.AcceptRelationshipReactivation;
using Backbone.Modules.Relationships.Module.Features.Relationships.CanEstablishRelationship;
using Backbone.Modules.Relationships.Module.Features.Relationships.CreateRelationship;
using Backbone.Modules.Relationships.Module.Features.Relationships.DecomposeRelationship;
using Backbone.Modules.Relationships.Module.Features.Relationships.GetRelationship;
using Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationships;
using Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationship;
using Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationshipReactivation;
using Backbone.Modules.Relationships.Module.Features.Relationships.RequestRelationshipReactivation;
using Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationship;
using Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationshipReactivation;
using Backbone.Modules.Relationships.Module.Features.Relationships.TerminateRelationship;
using Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.CreateRelationshipTemplate;
using Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.DeleteRelationshipTemplate;
using Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.GetRelationshipTemplate;
using Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Relationships.Module;

public static class RelationshipsEndpointExtensions
{
    public static IEndpointRouteBuilder MapRelationshipsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var relationships = endpoints.MapVersionedEndpointGroup("Relationships", "Relationships", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        relationships.MapGetRelationshipEndpoint();
        relationships.MapAcceptRelationshipEndpoint();
        relationships.MapRejectRelationshipEndpoint();
        relationships.MapRevokeRelationshipEndpoint();
        relationships.MapTerminateRelationshipEndpoint();
        relationships.MapRequestRelationshipReactivationEndpoint();
        relationships.MapRevokeRelationshipReactivationEndpoint();
        relationships.MapAcceptRelationshipReactivationEndpoint();
        relationships.MapRejectRelationshipReactivationEndpoint();
        relationships.MapDecomposeRelationshipEndpoint();
        relationships.MapCreateRelationshipEndpoint();
        relationships.MapCanEstablishRelationshipEndpoint();
        relationships.MapListRelationshipsEndpoint();

        var relationshipTemplates = endpoints.MapVersionedEndpointGroup("RelationshipTemplates", "RelationshipTemplates", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        relationshipTemplates.MapGetRelationshipTemplateEndpoint();
        relationshipTemplates.MapCreateRelationshipTemplateEndpoint();
        relationshipTemplates.MapDeleteRelationshipTemplateEndpoint();
        relationshipTemplates.MapListRelationshipTemplatesEndpoint();

        endpoints.MapListPublicRelationshipTemplateReferencesEndpoint();
        return endpoints;
    }
}
