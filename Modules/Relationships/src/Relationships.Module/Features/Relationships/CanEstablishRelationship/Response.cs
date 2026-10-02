namespace Backbone.Modules.Relationships.Module.Features.Relationships.CanEstablishRelationship;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CanEstablishRelationshipResponse")]
public class Response
{
    public required bool CanCreate { get; set; }
    public required string? Code { get; set; }
}
